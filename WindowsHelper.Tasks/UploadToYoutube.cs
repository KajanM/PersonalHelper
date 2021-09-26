using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;
using WindowsHelper.Services.Notion;
using WindowsHelper.Services.Notion.BindingModels;
using WindowsHelper.Services.Windows;
using WindowsHelper.Shared;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public class UploadToYoutube
    {
        private FileInfo _currentlyUploadingVideo;
        private int currentCredentialsIndex;
        private CurrentCourseDetails _currentCourseDetails;

        private YouTubeService _youtubeService;
        private UploadToYoutubeOptions _options;
        private readonly bool _isBulkUpload;
        private readonly GoogleSettings _googleSettings;
        private readonly INotionService _notionService;
        private readonly NotionSettings _notionSettings;

        private static readonly IReadOnlyCollection<string> TokenRelatedIssueIdentifiers = new List<string>
        {
            "quotaExceeded",
            "invalid_grant",
            "usageLimits"
        };

        public UploadToYoutube(UploadToYoutubeOptions options, GoogleSettings googleSettings,
            NotionSettings notionSettings)
        {
            if (options.IsBulkUpload)
            {
                _options = options;
                _isBulkUpload = true;
            }
            else
            {
                // take options from the meta file if exists
                _options = GetOptionsFromMetaFileAsync(Path.Join(options.Path,
                               GenerateUploadMetaTemplateFileOptions.DefaultMetaFileName)).Result
                           ?? options;
            }
            currentCredentialsIndex = _options.CredentialIndexToStartFrom;
            
            _googleSettings = googleSettings;
            _notionSettings = notionSettings;

            InitializeYoutubeService();

            _notionService = new NotionService(_notionSettings);
        }
        
        public async Task<int> ExecuteAsync()
        {
            var directoriesToUpload = await GetDirectoriesToUploadAsync();

            foreach (var directory in directoriesToUpload)
            {
                Log.Information("Uploading videos from {DirectoryName}", directory.FullName);
                _currentCourseDetails = new CurrentCourseDetails();
                if (_isBulkUpload)
                {
                    _options = GetOptionsFromMetaFileAsync(Path.Join(directory.FullName,
                        GenerateUploadMetaTemplateFileOptions.DefaultMetaFileName)).Result;
                    _options ??= new UploadToYoutubeOptions();
                }
                _options.Path = directory.FullName;
                await ProcessVideosInDirectoryAsync(directory);
            }

            if (_options.DoShutDown)
            {
                WindowsService.Shutdown(5);
            }

            return 1;
        }

        private async Task<IEnumerable<DirectoryInfo>> GetDirectoriesToUploadAsync()
        {
            var directoriesToUpload = new List<DirectoryInfo>();
            if (!string.IsNullOrWhiteSpace(_options.BulkUploadInputPath))
            {
                if (!File.Exists(_options.BulkUploadInputPath))
                    throw new ArgumentException($"{_options.BulkUploadInputPath} does not exist");

                var paths = await File.ReadAllLinesAsync(_options.BulkUploadInputPath);
                directoriesToUpload.AddRange(paths.Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(path => new DirectoryInfo(path)));
            }
            else
            {
                directoriesToUpload.Add(new DirectoryInfo(_options.Path));
            }

            return directoriesToUpload;
        }

        private async Task ProcessVideosInDirectoryAsync(DirectoryInfo directory)
        {
            _currentCourseDetails.PlaylistTitle = directory.Name;
            _currentCourseDetails.PlaylistId = GetPlaylistId();

            var uploadedDirectoryPath = Path.Join(directory.FullName, "uploaded");
            if (!Directory.Exists(uploadedDirectoryPath))
            {
                Directory.CreateDirectory(uploadedDirectoryPath);
            }

            Task addToNotionTask = null;
            if (_options.ShouldAddEntryToNotion)
            {
                addToNotionTask = AddToNotionAsync(); // task is awaited later
            }

            var videosToUpload = directory.GetVideos().ToList();
            Log.Information("Found {VideosCount} videos to upload", videosToUpload.Count);

            for (var i = 0; i < videosToUpload.Count; i++)
            {
                var videoToUpload = videosToUpload[i];

                _currentlyUploadingVideo = videoToUpload;
                try
                {
                    var description = GetDescriptionAsync(Path.GetFileNameWithoutExtension(videoToUpload.Name)).Result;
                    var (uploadProgress, _) = UploadAsync(videoToUpload.FullName, description).Result;
                    Log.Information("Upload status of {VideoName}: {UploadStatus}", videoToUpload.Name,
                        uploadProgress.Status);
                    if (uploadProgress.Status == UploadStatus.Completed)
                    {
                        MoveVideoToUploadedDirectory(videoToUpload.FullName);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while uploading {VideoName}. Exception:\n{@Exception}",
                        videoToUpload.Name, e);

                    if (IsRelatedToCurrentCredentials(e))
                    {
                        BypassCredentialsError();
                        i -= 1; // let's try uploading current video again
                    }
                }
            }

            if (_options.ShouldAddEntryToNotion)
            {
                await addToNotionTask;
            }
        }

        private static void MoveVideoToUploadedDirectory(string inputPath)
        {
            var outputPath = Path.Join(Path.GetDirectoryName(inputPath), "uploaded", Path.GetFileName(inputPath));
            Log.Information("Moving {@InputPath} to {@OutputPath}", inputPath, outputPath);
            File.Move(inputPath, outputPath);
        }

        private static bool IsRelatedToCurrentCredentials(Exception exception)
        {
            return TokenRelatedIssueIdentifiers.Any(identifier => exception.ToString().Contains(identifier));
        }

        private string GetPlaylistId()
        {
            if (!string.IsNullOrWhiteSpace(_options.PlaylistId)) return _options.PlaylistId;

            try
            {
                return _options.DoesPlaylistAlreadyExist
                    ? FindPlaylist(_currentCourseDetails.PlaylistTitle).Id
                    : CreatePlaylist(_currentCourseDetails.PlaylistTitle).Id;
            }
            catch (Exception e)
            {
                Log.Error("An error occured while initializing playlist id. {@Exception}", e);
                if (IsRelatedToCurrentCredentials(e))
                {
                    BypassCredentialsError();
                    return GetPlaylistId();
                }
            }

            Log.Warning("Unable to initialize playlist id");
            return null;
        }

        private void BypassCredentialsError()
        {
            currentCredentialsIndex += 1;
            if (currentCredentialsIndex >= _googleSettings.Credentials.Count)
                throw new ApplicationException($"Credential limit exceeded({currentCredentialsIndex})");

            Log.Information("Initializing Youtube service with credentials index {CredentialsIndex}",
                currentCredentialsIndex);
            InitializeYoutubeService();
        }

        private static async Task<UploadToYoutubeOptions> GetOptionsFromMetaFileAsync(string metaFilePath)
        {
            if (!File.Exists(metaFilePath)) return null;

            return (await File.ReadAllTextAsync(metaFilePath)).DeserializeYaml<UploadToYoutubeOptions>();
        }

        private async Task AddToNotionAsync()
        {
            if (string.IsNullOrWhiteSpace(_currentCourseDetails.PlaylistId))
            {
                Log.Warning("Skipping adding entry to Notion since playlist-id is not initialized");
                return;
            }

            await _notionService.AddCourseEntryAsync(new AddNewCourseRequestBindingModel(
                _notionSettings.CoursesDatabaseId,
                _options.Url,
                $"https://www.youtube.com/playlist?list={_currentCourseDetails.PlaylistId}",
                _currentCourseDetails.PlaylistTitle
            ));
        }

        private async Task<string> GetDescriptionAsync(string videoName)
        {
            var descriptionFileName = Path.Join(_options.Path, $"{videoName}.txt");
            if (!File.Exists(descriptionFileName)) return null;

            return await File.ReadAllTextAsync(descriptionFileName);
        }

        private Playlist FindPlaylist(string title)
        {
            Log.Information("Attempting to find playlist by title ({0})", title);
            var playlistRequest = _youtubeService.Playlists.List("id,snippet");
            playlistRequest.Mine = true;
            playlistRequest.MaxResults = 1000;

            var playlistResponse = playlistRequest.Execute();
            Log.Debug("Playlist list response: {@PlaylistResponse}", playlistResponse);
            var allPlaylists = playlistResponse.Items.ToList();
            Log.Information("Received a total of {PlaylistCount} playlists", allPlaylists.Count);
            var playlist = allPlaylists.FirstOrDefault(playlist => playlist.Snippet.Title == title);
            if (playlist == null) throw new ApplicationException($"Unable to find playlist with title {title}");
            Log.Information("Playlist Id is {PlaylistId}", playlist.Id);

            return playlist;
        }

        private Playlist CreatePlaylist(string title)
        {
            Log.Information("Creating new playlist with title {Title}", title);
            var newPlaylist = new Playlist
            {
                Snippet = new PlaylistSnippet { Title = title },
                Status = new PlaylistStatus { PrivacyStatus = "private" }
            };
            newPlaylist = _youtubeService.Playlists.Insert(newPlaylist, "snippet,status").Execute();
            Log.Information("Successfully created playlist. Id: {PlaylistId}", newPlaylist.Id);

            return newPlaylist;
        }

        private async Task<PlaylistItem> AddVideoToPlaylistAsync(string videoId)
        {
            Log.Information("Adding video (Id: {VideoId}, Name: {VideoName}) to playlist (Id: {PlaylistId})", videoId,
                _currentlyUploadingVideo.Name, _currentCourseDetails.PlaylistId);

            var newPlaylistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = _currentCourseDetails.PlaylistId,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            try
            {
                newPlaylistItem = await _youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
            }
            catch (Exception e)
            {
                Log.Error("An error occured while adding video ({Id}, {Name}) to playlist {PlayListId}. Exception\n{@Exception}",
                    videoId, _currentlyUploadingVideo.Name, _currentCourseDetails.PlaylistId);
                if (IsRelatedToCurrentCredentials(e))
                {
                    BypassCredentialsError();
                    return await AddVideoToPlaylistAsync(videoId); // try again
                }
            }

            return newPlaylistItem;
        }

        private async Task<UserCredential> GetCredentialAsync()
        {
            if (currentCredentialsIndex >= _googleSettings.Credentials.Count)
            {
                throw new ArgumentException(
                    $"Tried to get {currentCredentialsIndex}, but only {_googleSettings.Credentials.Count} provided.");
            }

            var credential = _googleSettings.Credentials[currentCredentialsIndex];

            if (string.IsNullOrWhiteSpace(credential?.ClientId))
                throw new ArgumentNullException($"Youtube credentials not initialized for {currentCredentialsIndex}");

            Log.Information("Using credential {CredentialIndex}", currentCredentialsIndex);

            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = credential.ClientId,
                    ClientSecret = credential.ClientSecret
                },
                RefreshGoogleTokens.GoogleProjectScopes,
                "user",
                CancellationToken.None,
                new FileDataStore(GoogleTokenHelper.GetTokenDirectoryPath(_options.Profile, currentCredentialsIndex))
            ).Result;
        }

        private async Task<(IUploadProgress progress, Video video)> UploadAsync(string filePath,
            string description = null)
        {
            var title = Path.GetFileNameWithoutExtension(filePath);
            Log.Information("Starting to upload\n Title: {Title}\nDescription: {Description}", title, description);

            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = title,
                    Description = description,
                },
                Status = new VideoStatus
                {
                    PrivacyStatus = "private"
                }
            };

            await using var fileStream = new FileStream(filePath, FileMode.Open);
            var videosInsertRequest = _youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            return (videosInsertRequest.UploadAsync().Result, video);
        }

        private void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Log.Information("{UploadedSize} of {FileSize} MB sent.", Utils.ToMb(progress.BytesSent),
                        Utils.ToMb(_currentlyUploadingVideo.Length));
                    break;
                case UploadStatus.Failed:
                    throw progress.Exception; // handled in calling logic
            }
        }

        private void videosInsertRequest_ResponseReceived(Video video)
        {
            Log.Information("Video id '{VideoId}'({Title}) was successfully uploaded.", video.Id, video.Snippet.Title);
            AddVideoToPlaylistAsync(video.Id).Wait();
        }

        private void InitializeYoutubeService()
        {
            var credential = GetCredentialAsync().Result;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
            _youtubeService.HttpClient.Timeout = TimeSpan.FromMinutes(3);
        }

        class CurrentCourseDetails
        {
            public string PlaylistId { get; set; }
            public string PlaylistTitle { get; set; }
        }
    }
}
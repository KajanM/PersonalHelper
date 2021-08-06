using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Notion;
using WindowsHelper.Services.Notion.BindingModels;
using WindowsHelper.Services.Windows;
using WindowsHelper.Shared;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Serilog;

namespace WindowsHelper.Tasks
{
    public class UploadToYoutube
    {
        private string playListId;
        private string playListTitle;
        private FileInfo currentlyUploadingVideo;
        
        private readonly YouTubeService _youtubeService;
        private readonly UploadToYoutubeOptions _options;
        private readonly YoutubeSettings _youtubeSettings;
        private readonly INotionService _notionService;
        private readonly NotionSettings _notionSettings;

        public UploadToYoutube(UploadToYoutubeOptions options, YoutubeSettings youtubeSettings, NotionSettings notionSettings)
        {
            _options = options;
            _youtubeSettings = youtubeSettings;
            _notionSettings = notionSettings;

            var credential = GetCredentialAsync().Result;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            _notionService = new NotionService(_notionSettings);
        }
        
        public async Task<int> ExecuteAsync()
        {
            var currentDirectory = new DirectoryInfo(_options.Path);
            var shouldAddEntryToNotion = !_options.DoesPlaylistAlreadyExist;
            playListTitle = currentDirectory.Name;

            playListId = _options.PlaylistId ?? (
                _options.DoesPlaylistAlreadyExist
                    ? FindPlaylist(playListTitle).Id
                    : CreatePlaylist(playListTitle).Id
            );

            Task addToNotionTask = null;
            if (shouldAddEntryToNotion)
            {
               addToNotionTask = AddToNotionAsync();
            }
            
            var videosToUpload = currentDirectory.GetVideos().ToList();
            Log.Information("Found {0} videos to upload", videosToUpload.Count);

            foreach (var videoToUpload in videosToUpload)
            {
                currentlyUploadingVideo = videoToUpload;
                try
                {
                    var description = GetDescriptionAsync(Path.GetFileNameWithoutExtension(videoToUpload.Name)).Result;
                    var (uploadProgress, uploadedVideo) = await UploadAsync(videoToUpload.FullName, description);
                    Log.Information("Upload status of {0}: {1}", videoToUpload.Name, uploadProgress.Status);
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while uploading {0}. Exception:\n{1}", videoToUpload.Name, e);
                }
            }

            if (shouldAddEntryToNotion)
            {
                await addToNotionTask;
            }

            if (_options.DoShutDown)
            {
                WindowsService.Shutdown();
            }
            
            return 1;
        }

        private async Task AddToNotionAsync()
        {
            if (string.IsNullOrWhiteSpace(playListId))
            {
                Log.Warning("Skipping adding entry to Notion since playlist-id is not initialized");
                return;
            }

            await _notionService.AddCourseEntryAsync(new AddNewCourseRequestBindingModel(
                _notionSettings.CoursesDatabaseId,
                _options.Url,
                $"https://www.youtube.com/playlist?list={playListId}",
                playListTitle
            ));
        }

        private static async Task<string> GetDescriptionAsync(string videoName)
        {
            var descriptionFileName = $"{videoName}.txt";
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
            Log.Debug("Playlist list response: {@0}", playlistResponse);
            var allPlaylists = playlistResponse.Items.ToList();
            Log.Information("Received a total of {0} playlists", allPlaylists.Count);
            var playlist = allPlaylists.FirstOrDefault(playlist => playlist.Snippet.Title == title);
            if (playlist == null) throw new ApplicationException($"Unable to find playlist with title {title}");
            Log.Information("Playlist Id is {0}", playlist.Id);
            
            return playlist;
        }
        
        private Playlist CreatePlaylist(string title)
        {
            Log.Information("Creating new playlist with title {0}", title);
            var newPlaylist = new Playlist
            {
                Snippet = new PlaylistSnippet {Title = title},
                Status = new PlaylistStatus {PrivacyStatus = "private"}
            };
            newPlaylist = _youtubeService.Playlists.Insert(newPlaylist, "snippet,status").Execute();
            Log.Information("Successfully created playlist. Id: {0}", newPlaylist.Id);

            return newPlaylist;
        }

        private async Task<PlaylistItem> AddVideoToPlaylistAsync(string videoId)
        {
            Log.Information("Adding video (Id: {0}, Name: {2}) to playlist (Id: {1})", videoId, playListId, currentlyUploadingVideo.Name);
            
            var newPlaylistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playListId,
                    ResourceId = new ResourceId {Kind = "youtube#video", VideoId = videoId}
                }
            };
            newPlaylistItem = await _youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            return newPlaylistItem;
        }

        private async Task<UserCredential> GetCredentialAsync()
        {
            var credential = PickCredentialsFromSettings();

            if (string.IsNullOrWhiteSpace(credential?.ClientId))
                throw new ArgumentNullException($"Youtube credentials not initialized for {_options.KeyPairToUse}");

            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = credential.ClientId,
                    ClientSecret = credential.ClientSecret
                },
                new[] {YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload},
                "user",
                CancellationToken.None,
                new FileDataStore(Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    _options.KeyPairToUse.ToString()))
            ).Result;
        }

        private YoutubeCredentials PickCredentialsFromSettings()
        {
            YoutubeCredentials credential = _options.KeyPairToUse switch
            {
                YoutubeKeyPair.KeyPairOne => _youtubeSettings.KeyPairOne,
                YoutubeKeyPair.KeyPairTwo => _youtubeSettings.KeyPairTwo,
                _ => null
            };

            return credential;
        }


        private async Task<(IUploadProgress progress, Video video)> UploadAsync(string filePath, string description = null)
        {
            var title = Path.GetFileNameWithoutExtension(filePath);
            Log.Information("Starting to upload\n Title: {0}\nDescription: {1}", title, description);
            
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
                    Log.Information("{0} of {1} MB sent.", Utils.ToMb(progress.BytesSent), Utils.ToMb(currentlyUploadingVideo.Length));
                    break;
                case UploadStatus.Failed:
                    Log.Error("An error prevented the upload of {1} from completing.\n{0}", progress.Exception, currentlyUploadingVideo.Name);
                    break;
            }
        }

        private void videosInsertRequest_ResponseReceived(Video video)
        {
            Log.Information("Video id '{0}'({1}) was successfully uploaded.", video.Id, video.Snippet.Title);
            AddVideoToPlaylistAsync(video.Id).Wait();
        }
    }
}
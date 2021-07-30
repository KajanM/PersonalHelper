using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
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
        
        private readonly YouTubeService _youtubeService;
        private readonly UploadToYoutubeOptions _options;

        public UploadToYoutube(UploadToYoutubeOptions options)
        {
            _options = options;
            
            var credential = GetCredentialAsync().Result;
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
        }
        
        public async Task<int> ExecuteAsync()
        {
            var currentDirectory = new DirectoryInfo(_options.Path);

            var playlist = _options.DoesPlaylistAlreadyExist
                ? FindPlaylist(currentDirectory.Name)
                : CreatePlaylist(currentDirectory.Name);
            
            playListId = playlist.Id;
            
            var videosToUpload = currentDirectory.GetVideos().ToList();

            foreach (var videoToUpload in videosToUpload)
            {
                Log.Information("Starting to upload {0}", videoToUpload.Name);
                try
                {
                    var (uploadProgress, uploadedVideo) = await UploadAsync(videoToUpload.FullName);
                    Log.Information("Upload status of {0}: {1}", videoToUpload.Name, uploadProgress.Status);
                }
                catch (Exception e)
                {
                    Log.Error("An error occured while uploading {0}. Exception:\n{1}", videoToUpload.Name, e);
                }
            }

            return 1;
        }

        private Playlist FindPlaylist(string title)
        {
            Log.Information("Attempting to find playlist by title ({0})", title);
            var playlistRequest = _youtubeService.Playlists.List("id,snippet");
            playlistRequest.Mine = true;
            playlistRequest.MaxResults = 1000;
            
            var playlistResponse = playlistRequest.Execute();
            Log.Debug("Playlist list response: {0}", playlistResponse);
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
            Log.Information("Adding video (Id: {0}) to playlist (Id: {1})", videoId, playListId);
            
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

        private static async Task<UserCredential> GetCredentialAsync()
        {
            var credentialFilePath = Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                "Config",
                "client_secret.json");

            if (!File.Exists(credentialFilePath))
            {
                throw new ApplicationException("Please add client_secret.json to the application root");
            }

            await using var stream =
                new FileStream(
                    credentialFilePath,
                    FileMode.Open, FileAccess.Read);

            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets,
                new[] {YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload},
                "user",
                CancellationToken.None,
                new FileDataStore(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            ).Result;
        }

        
        private async Task<(IUploadProgress progress, Video video)> UploadAsync(string filePath)
        {
            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = Path.GetFileNameWithoutExtension(filePath),
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

        private static void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Log.Information("{0} bytes sent.", progress.BytesSent);
                    break;
                case UploadStatus.Failed:
                    Log.Information("An error prevented the upload from completing.\n{0}", progress.Exception);
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
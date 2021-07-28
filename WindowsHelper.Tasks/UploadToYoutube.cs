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
    public static class UploadToYoutube
    {
        public static async Task<int> UploadAllAsync(UploadToYoutubeOptions options)
        {
            var credential = await GetCredentialAsync();

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var currentDirectory = new DirectoryInfo(options.Path);
            var videosToUpload = currentDirectory.GetVideos().ToList();

            foreach (var videoToUpload in videosToUpload)
            {
                Log.Information("Starting to upload {0}", videoToUpload.Name);
                var uploadedVideo = await UploadAsync(videoToUpload.FullName, youtubeService);
                Log.Information("Upload status of {0}: {1}", videoToUpload.Name, uploadedVideo.Status);
            }

            return 1;
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
                GoogleClientSecrets.Load(stream).Secrets,
                new[] {YouTubeService.Scope.YoutubeUpload},
                "user",
                CancellationToken.None,
                new FileDataStore(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            ).Result;
        }

        
        private static async Task<IUploadProgress> UploadAsync(string filePath, YouTubeService youtubeService)
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
            var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
            videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
            videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

            return videosInsertRequest.UploadAsync().Result;
        }

        private static void videosInsertRequest_ProgressChanged(IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Log.Information("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Completed:
                    Log.Information("Upload completed");
                    break;

                case UploadStatus.Failed:
                    Log.Information("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        private static void videosInsertRequest_ResponseReceived(Video video)
        {
            Log.Information("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
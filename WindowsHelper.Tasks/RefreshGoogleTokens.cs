using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Shared;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public class RefreshGoogleTokens
    {
        private readonly RefreshGoogleTokenOptions _options;
        private readonly YoutubeSettings _youtubeSettings;

        public RefreshGoogleTokens(RefreshGoogleTokenOptions options, YoutubeSettings youtubeSettings)
        {
            _options = options;
            _youtubeSettings = youtubeSettings;
        }
        
        public void Execute()
        {
            _options.EndIndex ??= _youtubeSettings.Credentials.Count;
            
            for (var i = _options.StartIndex; i < _options.EndIndex; i++)
            {
                var credentialDirectoryPath = GoogleTokenHelper.GetTokenDirectoryPath(_options.Profile, i);
                DeleteOldTokenIfExists(credentialDirectoryPath);
                
                GetCredentialAsync(i, credentialDirectoryPath).Wait();
            }
        }

        private static void DeleteOldTokenIfExists(string credentialDirectoryPath)
        {
            if (!Directory.Exists(credentialDirectoryPath)) return;
            
            foreach (var file in (new DirectoryInfo(credentialDirectoryPath)).GetFiles())
            {
                Log.Information("Deleting {FullName}", file.FullName);
                file.Delete();
            }
        }

        private async Task<UserCredential> GetCredentialAsync(int currentCredentialsIndex, string credentialDirectoryPath)
        {
            if (currentCredentialsIndex >= _youtubeSettings.Credentials.Count)
            {
                throw new ArgumentException(
                    $"Tried to get {currentCredentialsIndex}, but only {_youtubeSettings.Credentials.Count} provided.");
            }

            var credential = _youtubeSettings.Credentials[currentCredentialsIndex];

            if (string.IsNullOrWhiteSpace(credential?.ClientId))
                throw new ArgumentNullException($"Youtube credentials not initialized for {currentCredentialsIndex}");

            Log.Information("Refreshing credential {CredentialIndex}", currentCredentialsIndex);

            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = credential.ClientId,
                    ClientSecret = credential.ClientSecret
                },
                new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None,
                new FileDataStore(credentialDirectoryPath)
            ).Result;
        }
    }
}
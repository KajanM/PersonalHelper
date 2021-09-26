using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Shared;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public class DownloadFromGDrive
    {
        private DriveService _driveService;
        private readonly DownloadFromGDriveOptions _options;
        private readonly GoogleSettings _googleSettings;

        public DownloadFromGDrive(DownloadFromGDriveOptions options, GoogleSettings googleSettings)
        {
            _options = options;
            _googleSettings = googleSettings;

            InitializeDriveService();
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.Corpora = "drive";
                listRequest.IncludeTeamDriveItems = true;
                listRequest.SupportsAllDrives = true;
                listRequest.Fields = "id,name,mimeType";
                listRequest.DriveId = "0ANmrYYiQpsriUk9PVA";

                var response = listRequest.Execute();

                Log.Information("Response received: {@Response}", response);

                foreach (var file in response.Files)
                {
                    Log.Information("{@File}", file);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to get download links from g-drive");
            }
        }

        private void InitializeDriveService()
        {
            var credential = GetCredentialAsync().Result;
            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });
            _driveService.HttpClient.Timeout = TimeSpan.FromMinutes(3);
        }

        private async Task<UserCredential> GetCredentialAsync()
        {
            var credential = _googleSettings.Credentials[0];

            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = credential.ClientId,
                    ClientSecret = credential.ClientSecret
                },
                RefreshGoogleTokens.GoogleProjectScopes,
                "user",
                CancellationToken.None,
                new FileDataStore(GoogleTokenHelper.GetTokenDirectoryPath(_options.Profile, 0))
            ).Result;
        }
    }
}
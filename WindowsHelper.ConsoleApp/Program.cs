using System;
using System.Diagnostics;
using System.IO;
using WindowsHelper.Shared;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        public static AppSettings AppSettings;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"{DateTime.Now.Date:dd-MM-yyyy}.log")
                .CreateLogger();

            try
            {
                InitializeAppSettings();
                App.Start(args);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while processing");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitializeAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            var appSettingsConfig = config.GetSection("App");
            AppSettings = appSettingsConfig.Get<AppSettings>();
            AppSettings.YoutubeSettings = appSettingsConfig.GetSection("Youtube").Get<YoutubeSettings>();
            AppSettings.NotionSettings = appSettingsConfig.GetSection("Notion").Get<NotionSettings>();
        }
    }
}
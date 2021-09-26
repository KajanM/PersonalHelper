using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WindowsHelper.Shared;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        public static AppSettings AppSettings;

        static async Task Main(string[] args)
        {
            try
            {
                InitializeAppSettings();
                SetUpLogging();
                
                await App.StartAsync(args);
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

        private static void SetUpLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(LogEventLevel.Information)
                .WriteTo.File($"{DateTime.Now.Date:dd-MM-yyyy}.log")
                .WriteTo.Sentry(o =>
                {
                    o.Dsn = AppSettings.SentrySettings.Dsn;
                    o.MinimumBreadcrumbLevel = LogEventLevel.Warning;
                    o.MinimumEventLevel = LogEventLevel.Error;
                })
                .CreateLogger();
        }

        private static void InitializeAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            var appSettingsConfig = config.GetSection("App");
            AppSettings = appSettingsConfig.Get<AppSettings>();
            AppSettings.GoogleSettings = appSettingsConfig.GetSection("Google").Get<GoogleSettings>();
            AppSettings.NotionSettings = appSettingsConfig.GetSection("Notion").Get<NotionSettings>();
            AppSettings.SentrySettings = appSettingsConfig.GetSection("Sentry").Get<SentrySettings>();
        }
    }
}
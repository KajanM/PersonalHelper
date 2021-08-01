using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WindowsHelper.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        public static AppSettings AppSettings;

        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("panda.txt")
                .CreateLogger();
            Log.Logger = log;

            App.Start(args);

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    configuration
                        .SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    var configurationRoot = configuration.Build();
                    SetUpAppSettings(configurationRoot);
                });

        private static void SetUpAppSettings(IConfiguration configurationRoot)
        {
            var appSettingsConfig = configurationRoot.GetSection("App");
            AppSettings = appSettingsConfig.Get<AppSettings>();
            AppSettings.YoutubeSettings = appSettingsConfig.GetSection("Youtube").Get<YoutubeSettings>();
        }
    }
}
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
            using var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("panda.txt")
                .CreateLogger();
            Log.Logger = log;
            
            InitializeAppSettings();

            App.Start(args);
        }

        private static void InitializeAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            AppSettings = config.GetSection("App").Get<AppSettings>();
        }
    }
}
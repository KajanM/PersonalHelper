using Serilog;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("panda.txt")
                .CreateLogger();
            Log.Logger = log;

            App.Start(args);
        }
    }
}
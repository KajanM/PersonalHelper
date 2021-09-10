using System.IO;
using System.Text.RegularExpressions;
using Serilog;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public static class RemoveStringFromName
    {
        public static void Execute(RemoveStringFromNameOptions options)
        {
            var processedDirectoryPath = Path.Join(options.Path, "processed");
            if (!Directory.Exists(processedDirectoryPath))
            {
                Log.Information("Creating processed directory at {Path}", processedDirectoryPath);
                Directory.CreateDirectory(processedDirectoryPath);
            }
            foreach (var file in new DirectoryInfo(options.Path).GetFiles())
            {
                var newPath = Path.Join(processedDirectoryPath, Regex.Replace(file.Name, options.Pattern, ""));
                Log.Information("Copying {From} to {To}", file.FullName, newPath);
                File.Copy(file.FullName, newPath);
            }
        }
    }
}
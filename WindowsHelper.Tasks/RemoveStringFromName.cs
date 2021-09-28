using System.IO;
using System.Linq;
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

            var directory = new DirectoryInfo(options.Path);
            foreach (var file in directory.GetFiles())
            {
                var newPath = Path.Join(processedDirectoryPath, Regex.Replace(file.Name, options.Pattern, ""));
                Log.Information("Copying {From} to {To}", file.FullName, newPath);
                if (options.DoMove)
                {
                    File.Move(file.FullName, newPath, true);
                }
                else
                {
                    File.Copy(file.FullName, newPath, false);
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                var newPath = Path.Join(processedDirectoryPath, Regex.Replace(subDirectory.Name, options.Pattern, ""));
                Log.Information("Copying {From} to {To}", subDirectory.FullName, newPath);
                Directory.Move(subDirectory.FullName, newPath);
            }
        }
    }
}
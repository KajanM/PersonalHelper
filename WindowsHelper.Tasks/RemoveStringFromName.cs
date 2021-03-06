using System;
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
            var processedDirectoryPath =
                options.IgnoreUseProcessedDirectory ? options.Path : Path.Join(options.Path, "processed");
            if (!Directory.Exists(processedDirectoryPath))
            {
                Log.Information("Creating processed directory at {Path}", processedDirectoryPath);
                Directory.CreateDirectory(processedDirectoryPath);
            }

            var directory = new DirectoryInfo(options.Path);
            foreach (var subDirectory in directory.GetDirectories().Where(d => Regex.Match(d.Name, options.Pattern).Success))
            {
                var newPath = Path.Join(processedDirectoryPath, Regex.Replace(subDirectory.Name, options.Pattern, ""));
                if (string.IsNullOrWhiteSpace(newPath))
                {
                    throw new ArgumentException(nameof(options.Pattern));
                }
                Log.Information("Copying {From} to {To}", subDirectory.FullName, newPath);
                Directory.Move(subDirectory.FullName, newPath);
            }
              
            foreach (var file in directory.GetFiles().Where(d => Regex.Match(d.Name, options.Pattern).Success))
            {
                var newPath = Path.Join(processedDirectoryPath, Regex.Replace(file.Name, options.Pattern, ""));
                if (string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(newPath)))
                {
                    throw new ArgumentException(nameof(options.Pattern));
                }
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

          
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public static class AppendText
    {
        public static async Task ExecuteAsync(AppendTextOptions options)
        {
            await ProcessDirectoryAsync(options.Path, options);
        }

        private static async Task ProcessDirectoryAsync(string directoryPath, AppendTextOptions options)
        {
            await ProcessDirectoryAsync(new DirectoryInfo(directoryPath), options.Pattern, options.AppendText);
        }

        private static async Task ProcessDirectoryAsync(DirectoryInfo directory, string filePattern, string appendText)
        {
            foreach (var file in directory.GetFiles().Where(file => file.Name.Contains(filePattern)))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file.FullName);
                    if(content.Contains(appendText))
                    {
                        Log.Information("{File} already includes appended text. Skipping.", file.FullName);
                        continue;
                    }
                    
                    await File.AppendAllTextAsync(file.FullName, appendText);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to append text to {File}", file.FullName);
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                await ProcessDirectoryAsync(subDirectory, filePattern, appendText);
            }
        }
    }
}
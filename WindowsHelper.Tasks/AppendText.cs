using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public static class AppendText
    {
        public static async Task ExecuteAsync(AppendTextOptions options)
        {
            foreach (var file in Directory.GetFiles(options.Path, options.Pattern, SearchOption.AllDirectories))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(file);
                    if (content.Contains(options.AppendText))
                    {
                        Log.Information("{File} already includes appended text. Skipping.", file);
                        continue;
                    }

                    await File.AppendAllTextAsync(file, options.AppendText);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to append text to {File}", file);
                }
            }
        }
    }
}
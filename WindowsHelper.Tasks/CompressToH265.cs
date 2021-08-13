using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public static class CompressToH265
    {
        private const string OutputDirectory = "compressed";
        
        public static async Task ExecuteAsync(CompressToH265Options options)
        {
            var directory = new DirectoryInfo(options.Path);

            #region Create output directory if not exist
            
            var outputDirectoryAbsolutePath = Path.Join(directory.FullName, OutputDirectory);
            if (!File.Exists(outputDirectoryAbsolutePath))
            {
                Directory.CreateDirectory(outputDirectoryAbsolutePath);
            }

            #endregion

            var compressTasks = directory.GetVideos()
                .Where(video => !File.Exists(GetOutputFilePath(video.FullName)))
                .ToList()
                .Select(async video => await FfmpegCommandHelper.CompressAsync(video.FullName,
                    GetOutputFilePath(video.FullName),
                    options.CrfValue
                ))
                .ToList();

            var allTasks = Task.WhenAll(compressTasks);
            Log.Information("Found {Count} videos to compress", compressTasks.Count);
            try
            {
                allTasks.Wait();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while compressing the videos.");
            }
        }

        private static string GetOutputFilePath(string inputFilePath)
        {
            return Path.Join(Path.GetDirectoryName(inputFilePath), OutputDirectory, Path.GetFileName(inputFilePath));
        }
    }
}
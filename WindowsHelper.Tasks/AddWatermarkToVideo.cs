using System;
using System.IO;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public static class AddWatermarkToVideo
    {
        public static void Execute(AddWatermarkToVideoOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            var outputDirectory = Path.Join(directory.FullName, directory.Name.ReplaceInvalidChars());
            if (!Directory.Exists(outputDirectory))
            {
                Log.Information("Creating output directory at {Location}", outputDirectory);
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var video in directory.GetVideos())
            {
                try
                {
                    var outputPath = Path.Join(outputDirectory, video.Name);
                    if (File.Exists(outputPath))
                    {
                        Log.Information("{Video} is already processed. Skipping");
                        continue;
                    }

                    FfmpegCommandHelper.AddWatermark(video.FullName, outputPath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to add watermark to {File}", video.FullName);
                }
            }
        }
    }
}
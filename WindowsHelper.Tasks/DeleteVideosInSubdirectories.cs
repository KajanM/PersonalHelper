using System.IO;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;

namespace WindowsHelper.Tasks
{
    public static class DeleteVideosInSubdirectories
    {
        public static void Execute(DeleteVideosInSubdirectoriesOptions options)
        {
            var directory = new DirectoryInfo(options.Path);

            foreach (var subDirectory in directory.GetDirectories())
            {
                foreach (var video in subDirectory.GetVideos())
                {
                    Log.Information("Deleting {Path}", video.FullName);
                    if (options.IsDryRun) continue;
                    
                    File.Delete(video.FullName);
                }
            }
        }
    }
}
using System;
using System.IO;
using System.Linq;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;
using WindowsHelper.Tasks.Extensions;

namespace WindowsHelper.Tasks
{
    public class JoinVideosInInnerDirectoryAndMoveToParentDirectory
    {
        public static void Execute(JoinVideosInInnerDirectoryAndMoveToParentDirectoryOptions options)
        {
            var directory = new DirectoryInfo(options.Path);

            foreach (var subDirectory in directory.GetDirectories())
            {
                Log.Information("Processing {Path}", subDirectory.FullName);
                
                var joinOptions = new JoinMultipleVideosFfmpegOptions
                {
                    Path = subDirectory.FullName,
                    MaximumHourLimit = options.MaximumHourLimit,
                    NumberSeparatorChar = options.NumberSeparatorChar,
                    IsNumberAppended = options.IsNumberAppended,
                };

                var videos = subDirectory.GetVideos().ToList();
                if (videos.Count == 0)
                {
                    Log.Information("No videos found at {Path}. Skipping", subDirectory.FullName);
                    continue;
                }

                if (videos.Count == 1)
                {
                    Log.Information("Found only one video, just moving to parent directory");
                    var video = videos.FirstOrDefault();
                    var destinationPath = Path.Join(subDirectory.Parent.FullName, video.Name);
                    Log.Information("Moving {Source} to {Destination}", video.FullName, destinationPath);
                    File.Move(video.FullName, destinationPath);
                }
                
                try
                {
                    JoinMultipleVideosFfmpeg.JoinAsync(joinOptions).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while joining videos from {Path}", subDirectory.FullName);
                }

                try
                {
                    var jointFiles = subDirectory.GetFiles($"{subDirectory.Name.ReplaceInvalidChars()}*").ToList();
                    Log.Information("Found {Count} joint files", jointFiles.Count / 2);
                    
                    foreach (var jointFile in jointFiles)
                    {
                        var destinationPath = Path.Join(subDirectory.Parent.FullName, jointFile.Name);
                        Log.Information("Moving {Source} to {Destination}", jointFile.FullName, destinationPath);
                       File.Move(jointFile.FullName, destinationPath); 
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while moving the joint file from {SubDirectory}", subDirectory.FullName);
                }
            }
        }
    }
}
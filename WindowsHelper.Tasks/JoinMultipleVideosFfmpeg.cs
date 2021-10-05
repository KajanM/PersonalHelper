﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;
using Serilog;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Tasks
{
    public static class JoinMultipleVideosFfmpeg
    {
        public static async Task JoinAsync(JoinMultipleVideosFfmpegOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            options.OutputFileName ??= Path.GetFileName(options.Path).ReplaceInvalidChars();
            options.OutputExtension ??= DetermineOutputExtension(directory);

            if (!options.IsFileListAlreadyExist)
            {
                await GenerateTextFilesToBeConsumedByFfmpegAsync(directory, options);
            }

            var videoJoinTasks = directory.GetFiles($"{options.OutputFileName}-*-*.txt")
                .Select(ffmpegInputFile =>
                    Task.Run(async () => await FfmpegCommandHelper.ConcatMediaAsync(Path.Join(options.Path, ffmpegInputFile.Name),
                        Path.Join(options.Path, $"{Path.GetFileNameWithoutExtension(ffmpegInputFile.Name)}.{options.OutputExtension}"),
                        options.Path, options.IsDryRun)))
                .ToList();

            var allTasks = Task.WhenAll(videoJoinTasks);
            try
            {
                allTasks.Wait();
            }
            catch (Exception e)
            {
                Log.Information($"An error occured while joining the videos.{Environment.NewLine}{e}");
            }
        }

        private static string DetermineOutputExtension(DirectoryInfo directory)
        {
            var video = directory.GetVideos().FirstOrDefault();
            if(video == null) throw new InvalidOperationException("No video files found to automatically determine the output extension");

            return Path.GetExtension(video.Name)[1..];
        }

        private static async Task GenerateTextFilesToBeConsumedByFfmpegAsync(DirectoryInfo directory, JoinMultipleVideosFfmpegOptions options)
        {
            var fileQuery = directory.GetVideos();

            if (options.IsNumberAppended)
            {
                fileQuery = fileQuery.OrderBy(file => int.Parse(file.Name.Split(options.NumberSeparatorChar)[0]));
            }

            // group videos such that the group duration meets the input maximum-hour-limit
            var allVideos = fileQuery.ToList();
            var durationInSeconds = 0;
            var fileNames = new List<string>();
            var startCount = 1;

            for (var i = 0; i < allVideos.Count; i++)
            {
                var file = allVideos[i];
                var mediaDurationResult = await file.GetMediaDurationAsync();
                durationInSeconds += mediaDurationResult.seconds;
                fileNames.Add($"file '{file.Name}'");

                var doJoinAllVideos = options.MaximumHourLimit <= 0;
                var isMaximumHourLimitReached = durationInSeconds >= options.MaximumHourLimit * 60 * 60;
                var isLastVideo = i == allVideos.Count - 1;
                
                if ((!doJoinAllVideos && isMaximumHourLimitReached) || isLastVideo)
                { 
                    var videoListTextFileName = $"{options.OutputFileName}-{startCount:000}-{i + 1:000}.txt";
                    Log.Information($"Writing to {videoListTextFileName}");
                    await File.WriteAllLinesAsync(Path.Join(options.Path, videoListTextFileName), fileNames);
                    
                    startCount = i + 2;
                    fileNames.Clear();
                    durationInSeconds = 0;
                }
            }

        }
    }
}
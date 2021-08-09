using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;
using Serilog;

namespace WindowsHelper.Tasks
{
    public class JoinMultipleVideosFfmpeg
    {
        private readonly JoinMultipleVideosFfmpegOptions _options;

        public JoinMultipleVideosFfmpeg(JoinMultipleVideosFfmpegOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
            _options.OutputFileName = Path.GetFileName(_options.Path).ReplaceInvalidChars();
        }

        public async Task JoinAsync()
        {
            var directory = new DirectoryInfo(_options.Path);
            _options.OutputExtension ??= DetermineOutputExtension(directory);

            if (!_options.IsFileListAlreadyExist)
            {
                await GenerateTextFilesToBeConsumedByFfmpegAsync(directory);
            }

            var videoJoinTasks = directory.GetFiles($"{_options.OutputFileName}-*-*.txt")
                .Select(ffmpegInputFile =>
                    Task.Run(async () => await FfmpegCommandHelper.ConcatMediaAsync(ffmpegInputFile.Name,
                        $"{Path.GetFileNameWithoutExtension(ffmpegInputFile.Name)}.{_options.OutputExtension}",
                        _options.IsDryRun)))
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

        private async Task GenerateTextFilesToBeConsumedByFfmpegAsync(DirectoryInfo directory)
        {
            var fileQuery = directory.GetVideos();

            if (_options.IsNumberAppended)
            {
                fileQuery = fileQuery.OrderBy(file => int.Parse(file.Name.Split(_options.NumberSeparatorChar)[0]));
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

                var doJoinAllVideos = _options.MaximumHourLimit <= 0;
                var isMaximumHourLimitReached = durationInSeconds >= _options.MaximumHourLimit * 60 * 60;
                var isLastVideo = i == allVideos.Count - 1;
                
                if ((!doJoinAllVideos && isMaximumHourLimitReached) || isLastVideo)
                { 
                    var videoListTextFileName = $"{_options.OutputFileName}-{startCount}-{i + 1}.txt";
                    Log.Information($"Writing to {videoListTextFileName}");
                    await File.WriteAllLinesAsync(Path.Join(_options.Path, videoListTextFileName), fileNames);
                    
                    startCount = i + 2;
                    fileNames.Clear();
                    durationInSeconds = 0;
                }
            }

        }
    }
}
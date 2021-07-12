using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;
using CliWrap;
using CliWrap.Buffered;

namespace WindowsHelper.Tasks
{
    public class JoinMultipleVideosFfmpeg
    {
        private readonly JoinMultipleVideosFfmpegOptions _options;

        private static readonly List<string> VideoExtensions =
            new List<string>
                {
                    "mp4", "mov", "wmv", "ts", "avi", "webm"
                }
                .Select(e => $".{e}").ToList();

        public JoinMultipleVideosFfmpeg(JoinMultipleVideosFfmpegOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
        }

        public async Task JoinAsync()
        {
            var directory = new DirectoryInfo(_options.Path);

            if (!_options.IsFileListAlreadyExist)
            {
                await GenerateTextFilesToBeConsumedByFfmpegAsync(directory);
            }

            var videoJoinTasks = new List<Task<BufferedCommandResult>>();
            foreach (var ffmpegInputFile in directory.GetFiles($"{_options.OutputFileName}-*-*.txt"))
            {
                videoJoinTasks.Add(Task.Run(() => FfmpegCommandHelper.ConcatMediaAsync(ffmpegInputFile.Name,
                    $"{Path.GetFileNameWithoutExtension(ffmpegInputFile.Name)}.{_options.OutputExtension}",
                    _options.IsDryRun)));
            }

            var allTasks = Task.WhenAll(videoJoinTasks);
            try
            {
                allTasks.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured while joining the videos.{Environment.NewLine}{e}");
            }
        }

        private async Task GenerateTextFilesToBeConsumedByFfmpegAsync(DirectoryInfo directory)
        {
            var fileQuery = directory.GetFiles()
                .Where(file => VideoExtensions.Contains(Path.GetExtension(file.Name)))
                .AsEnumerable();

            if (_options.IsNumberAppended)
            {
                fileQuery = fileQuery.OrderBy(file => int.Parse(file.Name.Split("_")[0]));
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
                
                if (_options.MaximumHourLimit <= 0 || durationInSeconds >= _options.MaximumHourLimit * 60 * 60 || i == allVideos.Count - 1)
                { 
                    var videoListTextFileName = $"{_options.OutputFileName}-{startCount}-{i + 1}.txt";
                    Console.WriteLine($"Writing to {videoListTextFileName}");
                    await File.WriteAllLinesAsync(Path.Join(_options.Path, videoListTextFileName), fileNames);
                    
                    startCount = i + 2;
                    fileNames.Clear();
                    durationInSeconds = 0;
                }
            }

        }
    }
}
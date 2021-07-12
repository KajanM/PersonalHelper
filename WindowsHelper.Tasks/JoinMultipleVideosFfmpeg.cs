﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using CliWrap;
using CliWrap.Buffered;

namespace WindowsHelper.Tasks
{
    public class JoinMultipleVideosFfmpeg
    {
        private readonly JoinMultipleVideosFfmpegOptions _options;
        private const string VideoListFileName = "files";

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

            foreach (var ffmpegInputFile in directory.GetFiles("files-*.txt"))
            {
                await JoinUsingFfmpegAsync(ffmpegInputFile.Name, $"{Path.GetFileNameWithoutExtension(ffmpegInputFile.Name)}.{_options.OutputExtension}", _options.IsDryRun);
            }
        }

        private async Task JoinUsingFfmpegAsync(string inputFileName, string outputFileName, bool isDryRun)
        {
            Console.WriteLine($"Joining based on {inputFileName}");
            Console.WriteLine(await File.ReadAllTextAsync(inputFileName));
            
            if(isDryRun) return;

            var commandResult = Cli.Wrap("ffmpeg.exe")
                .WithArguments($"-f concat -i {inputFileName} -c copy {outputFileName}")
                .ExecuteBufferedAsync().Task.Result;

            Console.WriteLine(commandResult.StandardOutput);
            Console.WriteLine(commandResult.StandardError);
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
                    var videoListTextFileName = $"{VideoListFileName}-{startCount}-{i + 1}.txt";
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
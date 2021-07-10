using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WindowsHelper.ConsoleOptions;
using CliWrap;

namespace WindowsHelper.Tasks
{
    public class JoinMultipleVideosFfmpeg
    {
        private readonly JoinMultipleVideosFfmpegOptions _options;
        private const string VideoListFileName = "files.txt";

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

        public void Join()
        {
            var directory = new DirectoryInfo(_options.Path);

            if (!_options.IsFileListAlreadyExist)
            {
                GenerateTextFilesToBeConsumedByFfmpeg(directory);
            }

            Console.WriteLine("Joining the following files");
            Console.WriteLine(File.ReadAllText(VideoListFileName));

            if (_options.IsDryRun) return;

            JoinUsingFfmpeg(VideoListFileName);
        }

        private void JoinUsingFfmpeg(string inputFileName)
        {
            Console.WriteLine($"Joining based on {inputFileName}");
            
            var outBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var commandResult = Cli.Wrap("ffmpeg.exe")
                .WithArguments($"-f concat -i {inputFileName} -c copy {_options.OutputFileName}")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(outBuilder))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorBuilder))
                .ExecuteAsync()
                .Task.Result;

            Console.WriteLine("output");
            Console.WriteLine(outBuilder.ToString());

            Console.WriteLine("******************");
            Console.WriteLine("Error");
            Console.WriteLine(errorBuilder.ToString());
        }

        private void GenerateTextFilesToBeConsumedByFfmpeg(DirectoryInfo directory)
        {
            var fileQuery = directory.GetFiles()
                .Where(file => VideoExtensions.Contains(Path.GetExtension(file.Name)))
                .AsEnumerable();

            if (_options.IsNumberAppended)
            {
                fileQuery = fileQuery.OrderBy(file => int.Parse(file.Name.Split("-")[0]));
            }

            var lines = fileQuery.Select(file => $"file {file.Name}").ToList();

            File.WriteAllLines(Path.Join(_options.Path, VideoListFileName), lines);
        }
    }
}
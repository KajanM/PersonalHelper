using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;

namespace WindowsHelper.Tasks.Helpers
{
    public static class FfmpegCommandHelper
    {
        public static async Task<(int seconds, BufferedCommandResult cmdResult)> GetMediaDurationAsync(
            string filePathWithName)
        {
            Console.WriteLine($"Attempting to get duration of {filePathWithName}");
            
            // for some reason using await crashes the app sometimes
            var commandResult = Cli.Wrap("ffprobe.exe")
                .WithArguments(
                    $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePathWithName}\"")
                .ExecuteBufferedAsync().Task.Result;

            var isParseableString = float.TryParse(commandResult.StandardOutput, out var seconds);

            if (!isParseableString)
                throw new ArgumentException(
                    $"Unable to get media duration{Environment.NewLine}Seconds string: {commandResult.StandardError}");

            var nearestSecondsInteger = (int) Math.Ceiling(seconds);

            Console.WriteLine($"{nearestSecondsInteger} seconds");

            return (nearestSecondsInteger, commandResult);
        }

        public static async Task<BufferedCommandResult> ConcatMediaAsync(string inputFileName, string outputFileName,
            bool optionsIsDryRun)
        {
            Console.WriteLine($"Joining based on {inputFileName}");
            Console.WriteLine(await File.ReadAllTextAsync(inputFileName));

            if (optionsIsDryRun) return null;
            
            var commandResult = Cli.Wrap("ffmpeg.exe")
                .WithArguments($"-f concat -i {inputFileName} -c copy {outputFileName}")
                .ExecuteBufferedAsync().Task.Result;

            Console.WriteLine(commandResult.StandardOutput);
            Console.WriteLine(commandResult.StandardError);

            return commandResult;
        }
    }
}
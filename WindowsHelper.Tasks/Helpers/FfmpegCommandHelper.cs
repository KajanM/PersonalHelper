using System;
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
            var commandResult = await Cli.Wrap("ffprobe.exe")
                .WithArguments(
                    $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 {filePathWithName}")
                .ExecuteBufferedAsync();

            var isParseableString = float.TryParse(commandResult.StandardOutput, out var seconds);

            if (!isParseableString)
                throw new ArgumentException(
                    $"Unable to get media duration{Environment.NewLine}Seconds string: {commandResult.StandardOutput}");

            var nearestSecondsInteger = (int) Math.Ceiling(seconds);

            return (nearestSecondsInteger, commandResult);
        }
    }
}
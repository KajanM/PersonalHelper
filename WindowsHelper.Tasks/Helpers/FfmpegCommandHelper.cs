using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Serilog;

namespace WindowsHelper.Tasks.Helpers
{
    public static class FfmpegCommandHelper
    {
        public static (bool isSuccess, BufferedCommandResult commandResult) AddWatermark(string videoPath, string outputPath, string watermarkImagePath = null)
        {
            var stopwatch = Stopwatch.StartNew();
            Log.Information("Attempting to add watermark to {Video}", videoPath);

            if (string.IsNullOrWhiteSpace(watermarkImagePath))
            {
                watermarkImagePath = Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    "resources", "icon.png");
            }

            BufferedCommandResult commandResult = null;

            try
            {
                commandResult = Cli.Wrap("ffmpeg.exe")
                    .WithArguments(new[]
                    {
                        "-i", videoPath, "-i", watermarkImagePath, "-filter_complex", "overlay=x=main_w-48:y=main_h-48",
                        outputPath
                    })
                    .ExecuteBufferedAsync().Task.Result;
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to add watermark to {Name}", videoPath);
                return (false, commandResult);
            }

            Log.Information("Processed {VideoName} in {Time} minutes", videoPath,
                stopwatch.ElapsedMilliseconds / (1000 * 60));

            return (false, commandResult);
        }
        
        public static async Task<(int seconds, BufferedCommandResult cmdResult)> GetMediaDurationAsync(
            string filePathWithName)
        {
            Log.Information($"Attempting to get duration of {filePathWithName}");
            
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

            Log.Information($"{nearestSecondsInteger} seconds");

            return (nearestSecondsInteger, commandResult);
        }

        public static async Task<BufferedCommandResult> ConcatMediaAsync(string inputFileName, string outputFileName,
            string workingDirectoryPath, bool optionsIsDryRun)
        {
            Log.Information($"Joining based on {inputFileName}");
            Log.Information(await File.ReadAllTextAsync(inputFileName));

            if (optionsIsDryRun) return null;
            
            var commandResult = Cli.Wrap("ffmpeg.exe")
                .WithWorkingDirectory(workingDirectoryPath)
                .WithArguments(new []{"-f", "concat", "-i", inputFileName, "-c", "copy", outputFileName})
                .ExecuteBufferedAsync().Task.Result;

            Log.Information(commandResult.StandardOutput);
            Log.Information(commandResult.StandardError);

            return commandResult;
        }

        public static async Task<BufferedCommandResult> CompressAsync(string videoPathToCompress, string outputFileName, int crfValue = 24)
        {
            var stopwatch = Stopwatch.StartNew();
            Log.Information($"Starting to compress {videoPathToCompress}");
            var commandResult = await Cli.Wrap("ffmpeg.exe")
                .WithArguments(new []{"-i", videoPathToCompress, "-vcodec", "libx265", "-crf", crfValue.ToString(), outputFileName})
                .ExecuteBufferedAsync();

            Log.Information("Compressed {InputName} into {OutputName} in {Elapsed} minutes",
                videoPathToCompress,
                outputFileName,
                stopwatch.ElapsedMilliseconds / (1000 * 60));

            var (inputDuration, _) = await GetMediaDurationAsync(videoPathToCompress);
            var (outputDuration, _) = await GetMediaDurationAsync(outputFileName);
            if (inputDuration != outputDuration)
            {
                Log.Error(
                    "Input duration ({inputDuration}s [{InputPath}]) does not match the output duration({outputDuration}s [{OutputPath}])",
                    inputDuration, videoPathToCompress, outputDuration, outputFileName);
            }
            
            return commandResult;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;
using WindowsHelper.Tasks.Helpers;

namespace WindowsHelper.Tasks
{
    public class CompressToH265
    {
        private readonly CompressToH265Options _options;
        private const string OutputDirectory = "compressed";
        private const string ProcessedDirectory = "processed";
        private readonly List<string> _videoPathsToCompressQueue;

        public CompressToH265(CompressToH265Options options)
        {
            _options = options;
            var directory = new DirectoryInfo(_options.Path);

            #region Create output directory if not exist

            var outputDirectoryAbsolutePath = Path.Join(directory.FullName, OutputDirectory);
            if (!Directory.Exists(outputDirectoryAbsolutePath))
            {
                Log.Information("Creating the output directory at {OutputDirectoryPath}", outputDirectoryAbsolutePath);
                Directory.CreateDirectory(outputDirectoryAbsolutePath);
            }

            var processedDirectoryAbsolutePath = Path.Join(directory.FullName, ProcessedDirectory);
            if (!Directory.Exists(processedDirectoryAbsolutePath))
            {
                Log.Information("Creating the processed directory at {ProcessedDirectoryPath}",
                    processedDirectoryAbsolutePath);
                Directory.CreateDirectory(processedDirectoryAbsolutePath);
            }

            #endregion

            _videoPathsToCompressQueue = directory.GetVideos()
                .Where(file => !File.Exists(GetOutputFilePath(file.FullName)))
                .OrderBy(file => file.Length)
                .Select(file => file.FullName)
                .ToList();

            Log.Information("Found {Count} videos to compress", _videoPathsToCompressQueue.Count);
        }

        public async Task ExecuteAsync()
        {
            if (!_videoPathsToCompressQueue.Any())
            {
                Log.Information("All videos from the queue are processed.");
                return;
            }

            var videoPathsToCompress = _videoPathsToCompressQueue.Take(_options.ParallelCount).ToList();

            Log.Information("Processing {@VideosToCompress} from the queue.", videoPathsToCompress);
            var compressTasks = videoPathsToCompress
                .Select(path => Task.Run(async () =>
                    await FfmpegCommandHelper.CompressAsync(path, GetOutputFilePath(path), _options.CrfValue)))
                .ToList();

            var allTasks = Task.WhenAll(compressTasks);
            try
            {
                allTasks.Wait();
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while compressing the videos.");
            }
            finally
            {
                Log.Information("Removing {@ProcessedVideos} from the queue.", videoPathsToCompress);
                videoPathsToCompress.ForEach(compressedPath =>
                {
                    _videoPathsToCompressQueue.Remove(compressedPath);
                    try
                    {
                       
                        var moveToPath = GetProcessedFilePath(compressedPath);
                        Log.Information("Moving {Path} to {MoveToPath}", compressedPath, moveToPath);
                        File.Move(compressedPath, moveToPath);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "An error occured while handling the compressed file");
                    }
                });
                await ExecuteAsync(); // process remaining items from the queue
            }
        }

        private static string GetProcessedFilePath(string inputFilePath)
        {
            return Path.Join(Path.GetDirectoryName(inputFilePath), ProcessedDirectory, Path.GetFileName(inputFilePath));
        }

        private static string GetOutputFilePath(string inputFilePath)
        {
            return Path.Join(Path.GetDirectoryName(inputFilePath), OutputDirectory, Path.GetFileName(inputFilePath));
        }
    }
}
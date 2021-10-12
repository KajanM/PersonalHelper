using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks;
using CommandLine;
using Serilog;
using WindowsHelper.Services.Windows;
using WindowsHelper.Tasks.Udemy;

namespace WindowsHelper.ConsoleApp
{
    public static class App
    {
        public static async Task StartAsync(string[] args)
        {
            var types = new[]
            {
                typeof(AppendNumberToFilesOptions), typeof(ChangeSystemTimeOptions),
                typeof(MoveToParentDirectoryOptions),
                typeof(ReplaceInvalidCharsFromFileNameOptions),
                typeof(JoinMultipleVideosFfmpegOptions),
                typeof(UploadToYoutubeOptions),
                typeof(GenerateUploadMetaTemplateFileOptions),
                typeof(CompressToH265Options),
                typeof(ShutdownOptions),
                typeof(CrawlUdemyCouponsOptions),
                typeof(DownloadFromLibgenOptions),
                typeof(AddToIdmQueueOptions),
                typeof(RefreshGoogleTokenOptions),
                typeof(UpdateFreshCouponsRepoOptions),
                typeof(ExtractZipOptions),
                typeof(AppendTextOptions),
                typeof(AddWatermarkToVideoOptions),
                typeof(RemoveStringFromNameOptions),
                typeof(DownloadFromGDriveOptions),
                typeof(JoinVideosInInnerDirectoryAndMoveToParentDirectoryOptions),
                typeof(AnalyzeVideosOptions),
                typeof(DeleteVideosInSubdirectoriesOptions),
            };
            var result = Parser.Default.ParseArguments(args, types);
            result.MapResult(async (object opts) => await RunAsync(opts),HandleParseErrorAsync);
        }

        static async Task<int> RunAsync(object options)
        {
            Log.Debug("Options received {Options}", options);

            try
            {
                switch (options)
                {
                    case AppendNumberToFilesOptions opts:
                        new FileOrganizer(opts).PrependFileNamesWithNumber();
                        return 0;
                    case ChangeSystemTimeOptions opts:
                        await WindowsTimeService.ExecuteAsync(opts);
                        return 0;
                    case MoveToParentDirectoryOptions opts:
                        new MoveToParentDirectory(opts).Move();
                        return 0;
                    case ReplaceInvalidCharsFromFileNameOptions opts:
                        ReplaceInvalidCharsFromFileName.Execute(opts);
                        return 0;
                    case JoinMultipleVideosFfmpegOptions opts:
                        await JoinMultipleVideosFfmpeg.JoinAsync(opts);
                        return 0;
                    case UploadToYoutubeOptions opts:
                        await new UploadToYoutube(opts, Program.AppSettings.GoogleSettings,
                            Program.AppSettings.NotionSettings).ExecuteAsync();
                        return 0;
                    case GenerateUploadMetaTemplateFileOptions opts:
                        await GenerateUploadMetaTemplateFile.ExecuteAsync(opts);
                        return 0;
                    case CompressToH265Options opts:
                        await new CompressToH265(opts).ExecuteAsync();
                        return 0;
                    case ShutdownOptions opts:
                        WindowsService.Shutdown(opts.Minutes, opts.Hours, opts.Seconds);
                        return 0;
                    case CrawlUdemyCouponsOptions opts:
                        new CrawlUdemyCoupons().ExecuteAsync().Wait();
                        return 0;
                    case DownloadFromLibgenOptions opts:
                        await new DownloadFromLibgen().ExecuteAsync(opts);
                        return 0;
                    case AddToIdmQueueOptions opts:
                        AddToIdmQueue.ExecuteAsync(opts);
                        return 0;
                    case RefreshGoogleTokenOptions opts:
                        new RefreshGoogleTokens(opts, Program.AppSettings.GoogleSettings).Execute();
                        return 0;
                    case UpdateFreshCouponsRepoOptions opts:
                        await UpdateFreshCouponsRepo.ExecuteAsync(opts);
                        return 0;
                    case ExtractZipOptions opts:
                        ExtractZip.Execute(opts);
                        return 0;
                    case AppendTextOptions opts:
                        await AppendText.ExecuteAsync(opts);
                        return 0;
                    case AddWatermarkToVideoOptions opts:
                        AddWatermarkToVideo.Execute(opts);
                        return 0;
                    case RemoveStringFromNameOptions opts:
                        RemoveStringFromName.Execute(opts);
                        return 0;
                    case DownloadFromGDriveOptions opts:
                        await new DownloadFromGDrive(opts, Program.AppSettings.GoogleSettings).ExecuteAsync();
                        return 0;
                    case JoinVideosInInnerDirectoryAndMoveToParentDirectoryOptions opts:
                        JoinVideosInInnerDirectoryAndMoveToParentDirectory.Execute(opts);
                        return 0;
                    case AnalyzeVideosOptions opts:
                        AnalyzeVideos.Execute(opts);
                        return 0;
                    case DeleteVideosInSubdirectoriesOptions opts:
                        DeleteVideosInSubdirectories.Execute(opts);
                        return 0;
                    default:
                        return -1;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while processing");
                return -1;
            }
        }

        static async Task<int> HandleParseErrorAsync(IEnumerable<Error> errs)
        {
            Log.Error(string.Join(Environment.NewLine, errs));
            return 1;
        }
    }
}
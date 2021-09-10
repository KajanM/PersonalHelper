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
                typeof(RemoveStringFromNameOptions)
            };
            var result = Parser.Default.ParseArguments(args, types);
            result.MapResult(async (object opts) => await RunAsync(opts),HandleParseErrorAsync);
        }

        static async Task<int> RunAsync(object options)
        {
            Log.Debug("Options received {Options}", options);

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
                    await new JoinMultipleVideosFfmpeg(opts).JoinAsync();
                    return 0;
                case UploadToYoutubeOptions opts:
                    await new UploadToYoutube(opts, Program.AppSettings.YoutubeSettings,
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
                    new RefreshGoogleTokens(opts, Program.AppSettings.YoutubeSettings).Execute();
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
                default:
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
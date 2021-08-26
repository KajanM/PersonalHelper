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
            await Parser.Default.ParseArguments<AppendNumberToFilesOptions, ChangeSystemTimeOptions,
                    MoveToParentDirectoryOptions,
                    ReplaceInvalidCharsFromFileNameOptions,
                    JoinMultipleVideosFfmpegOptions,
                    UploadToYoutubeOptions,
                    GenerateUploadMetaTemplateFileOptions,
                    CompressToH265Options,
                    ShutdownOptions,
                    CrawlUdemyCouponsOptions,
                    DownloadFromLibgenOptions,
                    AddToIdmQueueOptions,
                    RefreshGoogleTokenOptions
                >(args)
                .MapResult(
                    async (AppendNumberToFilesOptions opts) => new FileOrganizer(opts).PrependFileNamesWithNumber(),
                    async (ChangeSystemTimeOptions opts) => await WindowsTimeService.ExecuteAsync(opts),
                    async (MoveToParentDirectoryOptions opts) => new MoveToParentDirectory(opts).Move(),
                    async (ReplaceInvalidCharsFromFileNameOptions opts) => new ReplaceInvalidCharsFromFileName(opts).Replace(),
                    async (JoinMultipleVideosFfmpegOptions opts) => await new JoinMultipleVideosFfmpeg(opts).JoinAsync(),
                    async (UploadToYoutubeOptions opts) => await new UploadToYoutube(opts,
                        Program.AppSettings.YoutubeSettings, Program.AppSettings.NotionSettings).ExecuteAsync(),
                    async (GenerateUploadMetaTemplateFileOptions opts) => await GenerateUploadMetaTemplateFile.ExecuteAsync(opts),
                    async (CompressToH265Options opts) => await new CompressToH265(opts).ExecuteAsync(),
                    async (ShutdownOptions opts) => WindowsService.Shutdown(opts.Minutes, opts.Hours, opts.Seconds),
                    async (CrawlUdemyCouponsOptions opts) => await new CrawlUdemyCoupons().ExecuteAsync(),
                    async (DownloadFromLibgenOptions opts) => await new DownloadFromLibgen().ExecuteAsync(opts),
                    async (AddToIdmQueueOptions opts) => AddToIdmQueue.ExecuteAsync(opts),
                    async (RefreshGoogleTokenOptions opts) => new RefreshGoogleTokens(opts, Program.AppSettings.YoutubeSettings).Execute(),
                    HandleParseErrorAsync);
        }

        static async Task<int> HandleParseErrorAsync(IEnumerable<Error> errs)
        {
            Log.Error(string.Join(Environment.NewLine, errs));
            Console.ReadLine();

            return -1;
        }
    }
}
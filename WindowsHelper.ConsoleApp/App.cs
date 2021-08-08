using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks;
using CommandLine;
using Serilog;

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
                    UploadToYoutubeOptions
                >(args)
                .MapResult(
                    async (AppendNumberToFilesOptions opts) => await PrependFileNamesWithNumberAsync(opts),
                    async (ChangeSystemTimeOptions opts) => await ChangeSystemTimeAsync(opts),
                    async (MoveToParentDirectoryOptions opts) => await MoveToParentDirectoryAsync(opts),
                    async (ReplaceInvalidCharsFromFileNameOptions opts) => await ReplaceInvalidCharsAsync(opts),
                    async (JoinMultipleVideosFfmpegOptions opts) => await JoinVideosFfmpegAsync(opts),
                    async (UploadToYoutubeOptions opts) => await new UploadToYoutube(opts,
                        Program.AppSettings.YoutubeSettings, Program.AppSettings.NotionSettings).ExecuteAsync(),
                    HandleParseErrorAsync);
        }

        private static async Task<int> JoinVideosFfmpegAsync(JoinMultipleVideosFfmpegOptions opts)
        {
            var joiner = new JoinMultipleVideosFfmpeg(opts);
            await joiner.JoinAsync();
            return 1;
        }

        static async Task<int> ReplaceInvalidCharsAsync(ReplaceInvalidCharsFromFileNameOptions opts)
        {
            var replacer = new ReplaceInvalidCharsFromFileName(opts);

            replacer.Replace();

            return 1;
        }

        static async Task<int> MoveToParentDirectoryAsync(MoveToParentDirectoryOptions opts)
        {
            Log.Information($"{opts.Path}");

            var mover = new MoveToParentDirectory(opts);
            var isSuccess = mover.Move();

            return isSuccess ? 1 : -1;
        }

        static async Task<int> PrependFileNamesWithNumberAsync(AppendNumberToFilesOptions opts)
        {
            Log.Information($"{opts.Path}");
            var fileOrganizer = new FileOrganizer(opts);
            fileOrganizer.PrependFileNamesWithNumber();

            return 1;
        }

        static async Task<int> ChangeSystemTimeAsync(ChangeSystemTimeOptions opts)
        {
            try
            {
                var isSuccess = opts.IsReset
                    ? WindowsTimeService.ResetTime(opts).Result
                    : WindowsTimeService.ChangeTime(opts);

                return isSuccess ? 1 : -1;
            }
            catch (Exception e)
            {
                Log.Error("Unable to change the system time. {@opts} Exception: {e}", opts, e);
                return -1;
            }
        }

        static async Task<int> HandleParseErrorAsync(IEnumerable<Error> errs)
        {
            Log.Information(string.Join(Environment.NewLine, errs));
            Console.ReadLine();

            return -1;
        }
    }
}
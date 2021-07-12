using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks;
using CommandLine;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AppendNumberToFilesOptions, ChangeSystemTimeOptions, JoinMultipleVideosOptions,
                MoveToParentDirectoryOptions,
                ReplaceInvalidCharsFromFileNameOptions,
                JoinMultipleVideosFfmpegOptions
                >(args)
                .MapResult(
                    async (AppendNumberToFilesOptions opts) => await PrependFileNamesWithNumberAsync(opts),
                    async (ChangeSystemTimeOptions opts) => await ChangeSystemTimeAsync(opts),
                    async (JoinMultipleVideosOptions opts) => await JoinVideosAsync(opts),
                    async (MoveToParentDirectoryOptions opts) => await MoveToParentDirectoryAsync(opts),
                    async (ReplaceInvalidCharsFromFileNameOptions opts) => await ReplaceInvalidCharsAsync(opts),
                    async (JoinMultipleVideosFfmpegOptions opts) => await JoinVideosFfmpegAsync(opts),
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
            Console.WriteLine($"{opts.Path}");

            var mover = new MoveToParentDirectory(opts);
            var isSuccess = mover.Move();

            return isSuccess ? 1 : -1;
        }
        
        static async Task<int> JoinVideosAsync(JoinMultipleVideosOptions opts)
        {
            Console.WriteLine($"{opts.Path}");
            var joiner = new VideoJoiner(opts);

            var isSuccess = joiner.JoinVideosAsync().Result;

            return isSuccess ? 1 : -1;
        }

        static async Task<int> PrependFileNamesWithNumberAsync(AppendNumberToFilesOptions opts)
        {
            Console.WriteLine($"{opts.Path}");
            var fileOrganizer = new FileOrganizer(opts);
            fileOrganizer.PrependFileNamesWithNumber();

            return 1;
        }

        static async Task<int> ChangeSystemTimeAsync(ChangeSystemTimeOptions opts)
        {
            try
            {
                var isSuccess = opts.IsReset ? 
                    WindowsTimeService.ResetTime(opts).Result : 
                    WindowsTimeService.ChangeTime(opts);

                return isSuccess ? 1 : -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        static async Task<int> HandleParseErrorAsync(IEnumerable<Error> errs)
        {
            Console.WriteLine(string.Join(Environment.NewLine, errs));
            Console.ReadLine();

            return -1;
        }
    }
}
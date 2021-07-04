using System;
using System.Collections.Generic;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.OrganizeFileName;
using CommandLine;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AppendNumberToFilesOptions, ChangeSystemTimeOptions, JoinMultipleVideosOptions>(args)
                .MapResult(
                    (AppendNumberToFilesOptions opts) => PrependFileNamesWithNumber(opts),
                    (ChangeSystemTimeOptions opts) => ChangeSystemTime(opts),
                    (JoinMultipleVideosOptions opts) => JoinVideos(opts),
                    HandleParseError);
        }

        static int JoinVideos(JoinMultipleVideosOptions opts)
        {
            Console.WriteLine($"{opts.Path}");
            var joiner = new VideoJoiner(opts);

            var isSuccess = joiner.JoinVideosAsync().Result;

            return isSuccess ? 1 : -1;
        }

        static int PrependFileNamesWithNumber(AppendNumberToFilesOptions opts)
        {
            Console.WriteLine($"{opts.Path}");
            var fileOrganizer = new FileOrganizer(opts);
            fileOrganizer.PrependFileNamesWithNumber();

            return 1;
        }

        static int ChangeSystemTime(ChangeSystemTimeOptions opts)
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

        static int HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine(string.Join(Environment.NewLine, errs));
            Console.ReadLine();

            return -1;
        }
    }
}
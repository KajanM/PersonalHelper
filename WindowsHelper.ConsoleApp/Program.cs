using System;
using System.Collections.Generic;
using WindowsHelper.ConsoleOptions;
using CommandLine;
using FileHelper;

namespace WindowsHelper.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AppendNumberToFilesOptions>(args)
                .MapResult(
                    PrependFileNamesWithNumber,
                    HandleParseError);
        }

        static int PrependFileNamesWithNumber(AppendNumberToFilesOptions opts)
        {
            Console.WriteLine($"{opts.Path}");
            var fileOrganizer = new FileOrganizer(opts);
            fileOrganizer.PrependFileNamesWithNumber();

            return 1;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine(string.Join(Environment.NewLine, errs));
            Console.ReadLine();
            
            return -1;
        }
    }
}
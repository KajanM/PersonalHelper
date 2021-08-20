using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("libgen",
        HelpText =
            "Download from libgen.")]
    public class DownloadFromLibgenOptions
    {
        [Option('t',  Required = true, HelpText = "The search term.")]
        public string SearchTerm { get; set; }

        [Option('s', Required = false, HelpText = "Page number to start downloading from. Defaults to the first page.")]
        public int StartPageNumber { get; set; } = 1;

        [Option('c', Required = false, HelpText = "Number of pages to crawl and download. Defaults to 10.")]
        public int PageCountToDownload { get; set; } = 10;

        [Option('d', Required = false, HelpText = "Do download the file. Defaults to 'true'.")]
        public bool DoDownload { get; set; } = true;

        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
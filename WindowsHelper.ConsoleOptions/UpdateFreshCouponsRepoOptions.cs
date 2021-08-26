using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("fc-update",
        HelpText =
            "Update fresh coupons repo")]
    public class UpdateFreshCouponsRepoOptions
    {
        [Option('n', "name", Required = false, HelpText = "The file name. If not provided file with today's date will be picked.")]
        public string FileName { get; set; }
        
        [Option('p', "path", Required = false, HelpText = "The path. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('r', "r-path", Required = false, HelpText = "The repository path")]
        public string RepoPath { get; set; } = @"D:\experiments\fresh-coupons-data";

        [Option('d', "dry-run", Required = false, HelpText = "Is dry run")]
        public bool IsDryRun { get; set; }
    }
}
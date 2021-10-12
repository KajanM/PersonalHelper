using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("analyze",
        HelpText =
            "Analyze all videos in the directory")]
    public class AnalyzeVideosOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
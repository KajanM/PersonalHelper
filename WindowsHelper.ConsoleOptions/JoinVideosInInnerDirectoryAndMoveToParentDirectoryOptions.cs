using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("join-inner",
        HelpText =
            "Join all videos in the inner directory and move to parent directory")]
    public class JoinVideosInInnerDirectoryAndMoveToParentDirectoryOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
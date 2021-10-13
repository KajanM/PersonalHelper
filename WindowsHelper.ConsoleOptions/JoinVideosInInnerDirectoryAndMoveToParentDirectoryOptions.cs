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

        [Option('h', "hour-limit", Required = false,
            Default = 5,
            HelpText =
                "Maximum hour limit to determine the number of videos to join. Set to 0 if joining all the videos.")]
        public int MaximumHourLimit { get; set; } = 5;
        
        [Option("separator", Required = false,
            HelpText = "Number separator char used in the file name. Defaults to '-'.")]
        public string NumberSeparatorChar { get; set; } = "-";
    }
}
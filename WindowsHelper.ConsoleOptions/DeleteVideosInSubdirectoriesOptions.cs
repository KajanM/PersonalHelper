using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("delete-videos",
        HelpText =
            "Delete all videos")]
    public class DeleteVideosInSubdirectoriesOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
        
        [Option('d', "dry", Required = false, HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("join", HelpText = "Join all videos in a directory using ffmpeg")]
    public class JoinMultipleVideosFfmpegOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('o', "output-filename", Required = false, HelpText = "The output file name. Defaults to the parent directory name")]
        public string OutputFileName { get; set; }

        [Option('i', "ignore-list-generation", Required = false, HelpText = "Does input files for ffmpeg already exist")]
        public bool IsFileListAlreadyExist { get; set; }

        [Option('n', "is-number-appended", Required = false,
            HelpText = "Number is appended to the file name. Used to order the files. Defaults to true")]
        public bool IsNumberAppended { get; set; } = true;

        [Option("separator", Required = false,
            HelpText = "Number separator char used in the file name. Defaults to '-'.")]
        public string NumberSeparatorChar { get; set; } = "-";

        [Option('h', "hour-limit", Required = false,
            HelpText =
                "Maximum hour limit to determine the number of videos to join. Set to 0 if joining all the videos. Defaults to 1 hour.")]
        public int MaximumHourLimit { get; set; } = 1;

        [Option('e', "extension", Required = false,
            HelpText =
                "The output extension. If not specified, will be automatically set to the extension of a video in the directory.")]
        public string OutputExtension { get; set; }

        [Option('d', "dry", Required = false,
            HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
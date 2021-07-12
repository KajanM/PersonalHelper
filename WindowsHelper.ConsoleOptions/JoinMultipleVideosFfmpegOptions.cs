using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("join-ffmpeg", HelpText = "Join all videos in a directory using ffmpeg")]
    public class JoinMultipleVideosFfmpegOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; }

        [Option('o', "output-filename", Required = false, HelpText = "The output file name. Defaults to 'joint'")]
        public string OutputFileName { get; set; } = "joint";

        [Option('i', "ignore-list-generation", Required = false, HelpText = "Does input files for ffmpeg already exist")]
        public bool IsFileListAlreadyExist { get; set; }

        [Option('n', "is-number-appended", Required = false,
            HelpText = "Number is appended to the file name. Used to order the files")]
        public bool IsNumberAppended { get; set; }

        [Option('h', "hour-limit", Required = false,
            HelpText =
                "Maximum hour limit to determine the number of videos to join. Set to 0 if joining all the videos. Defaults to 2 hours.")]
        public int MaximumHourLimit { get; set; } = 2;

        [Option('e', "extension", Required = false,
            HelpText =
                "The output extension. Defaults to mp4")]
        public string OutputExtension { get; set; } = "mp4";

        [Option('d', "dry", Required = false,
            HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
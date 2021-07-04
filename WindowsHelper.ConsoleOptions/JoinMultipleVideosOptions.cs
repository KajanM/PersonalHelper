using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("join-video", HelpText = "Join multiple videos")]
    public class JoinMultipleVideosOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action")]
        public string Path { get; set; }

        [Option('v', "vlc-path", Required = false, HelpText = "VLC executable file path")]
        public string VlcPath { get; set; } = @"C:\Program Files\VideoLAN\VLC\vlc.exe";

        [Option('d', "dry", Required = false,
            HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("upload",
        HelpText =
            "Upload videos in the current directory to youtube, playlist will be created based on the directory name.")]
    public class UploadToYoutubeOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
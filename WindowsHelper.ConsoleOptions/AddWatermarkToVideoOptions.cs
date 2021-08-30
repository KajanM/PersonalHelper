using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("aw",
        HelpText =
            "add watermark to video")]
    public class AddWatermarkToVideoOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
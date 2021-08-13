using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("compress",
        HelpText =
            "Compress all videos in the directory using H265 codec")]
    public class CompressToH265Options
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('c', "crf", Required = false, HelpText = "The CRF value to use. Defaults to 24.")]
        public int CrfValue { get; set; } = 24;
    }
}
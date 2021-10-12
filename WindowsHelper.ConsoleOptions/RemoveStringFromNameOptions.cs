using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("remove",
        HelpText =
            "Remove particular text from file name")]
    public class RemoveStringFromNameOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option(Required = true, HelpText = "The pattern to replace")]
        public string Pattern { get; set; }

        [Option('i', "ignore-processed-dir", Required = false, Default = false,
            HelpText = "Do not use separate processed directory")]
        public bool IgnoreUseProcessedDirectory { get; set; }

        [Option(Required = false, Default = true, HelpText = "Should move the file instead of copying")]
        public bool DoMove { get; set; } = false;
    }
}
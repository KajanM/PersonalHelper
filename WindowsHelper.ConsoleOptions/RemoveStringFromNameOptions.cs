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
    }
}
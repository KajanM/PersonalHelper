using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("extract", HelpText = "Extract all zip files.")]
    public class ExtractZipOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
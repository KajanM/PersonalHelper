using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("prepend-number", HelpText = "Prepends file name with number from parent directory")]
    public class AppendNumberToFilesOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action")]
        public string Path { get; set; }

        [Option('d', "dry", Required = false, HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
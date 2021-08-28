using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("ric", HelpText = "Replace invalid char")]
    public class ReplaceInvalidCharsFromFileNameOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action")]
        public string Path { get; set; } = Environment.CurrentDirectory;
        
        [Option('d', "dry", Required = false, HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }

        [Option('l', "lower", Default = false, Required = false, HelpText = "Should not use lower case?")]
        public bool DoNotChangeToLower { get; set; } 
    }
}
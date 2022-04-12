using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("alter-db",
        HelpText =
            "Alter full sql export")]
    public class AlterDbOptions
    {
        [Option('n', "name", Required = true, HelpText = "(relative)file name")]
        public string FileName { get; set; } = "20220411_1042";
        
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory.")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("idm",
        HelpText =
            "Add links to IDM queue.")]
    public class AddToIdmQueueOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;
    }
}
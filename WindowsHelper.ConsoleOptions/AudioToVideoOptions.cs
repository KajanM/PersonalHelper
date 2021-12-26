using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("atv", HelpText = "Convert all audios to videos")]
    public class AudioToVideoOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('e', "ext", Required = false,
            HelpText = "Audio extension", Default = "mp3")]
        public string Extension { get; set; }
    }
}
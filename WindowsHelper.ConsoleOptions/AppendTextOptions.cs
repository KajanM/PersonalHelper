using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("append-text",
        HelpText =
            "Append text all files matching the pattern.")]
    public class AppendTextOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option(Required = false, HelpText = "Matching file name pattern. Defaults to App.*sx")]
        public string Pattern { get; set; } = "App.*sx";

        [Option('t', "text", Required = false, HelpText = "Text to append. Defaults to 'export default App'")]
        public string AppendText { get; set; } = "export default App";
    }
}
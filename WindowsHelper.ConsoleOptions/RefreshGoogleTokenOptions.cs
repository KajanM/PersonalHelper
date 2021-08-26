using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("refresh", HelpText = "Refresh google tokens")]
    public class RefreshGoogleTokenOptions
    {
        [Option('p', Required = false, HelpText = "The profile name. Defaults to 'kajan'.")]
        public string Profile { get; set; } = "kajan";

        [Option('s', Required = false, HelpText = "The start index, defaults to 0.")]
        public int StartIndex { get; set; }
        
        [Option('e', Required = false, HelpText = "The end index, defaults to the last token.")]
        public int? EndIndex { get; set; }
    }
}
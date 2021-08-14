using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("off",
        HelpText =
            "Shutdown computer")]
    public class ShutdownOptions
    {
        [Option('s', "sec", Required = false, HelpText = "Seconds.")]
        public int Seconds { get; set; }
        
        [Option('m', "min", Required = false, HelpText = "Minutes.")]
        public int Minutes { get; set; }

        [Option('h', "hr", Required = false, HelpText = "Hours. Defaults to 1.")]
        public int Hours { get; set; } = 1;
    }
}
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    public class BaseOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool IsVerbose { get; set; }
    }
}
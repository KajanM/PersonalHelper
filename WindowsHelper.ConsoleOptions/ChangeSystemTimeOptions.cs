using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("set-time", HelpText = "Change system date time")]
    public class ChangeSystemTimeOptions
    {
        [Option('y', "year", Required = false)]
        public short? Year { get; set; }
        
        [Option('M', "month", Required = false)]
        public short? Month { get; set; }
        
        [Option('d', "day", Required = false)]
        public short? Day { get; set; }
        
        [Option('h', "hour", Required = false)]
        public short? Hour { get; set; }
        
        [Option('m', "minute", Required = false)]
        public short? Minute { get; set; }
        
        [Option('s', "second", Required = false)]
        public short? Second { get; set; }
        
        [Option("millisecond", Required = false)]
        public short? Millisecond { get; set; }
        
        [Option('r', "reset", Required = false)]
        public bool IsReset { get; set; }
        
        
        
        [Option( "dry", Required = false, HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
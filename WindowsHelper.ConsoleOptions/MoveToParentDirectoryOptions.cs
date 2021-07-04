using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("move-up", HelpText = "Move all files to the parent directory")]
    public class MoveToParentDirectoryOptions
    {
        [Option('p', "path", Required = false, HelpText = "Path to perform action")]
        public string Path { get; set; } 
        
        [Option('c', "copy", Required = false, HelpText = "Copy instead of moving")]
        public bool IsCopy { get; set; } 
        
        
        [Option('d', "dry", Required = false, HelpText = "Prints the expected change without really affecting the file system")]
        public bool IsDryRun { get; set; }
    }
}
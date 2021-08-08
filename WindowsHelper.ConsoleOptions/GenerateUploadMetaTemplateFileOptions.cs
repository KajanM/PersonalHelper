using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    
    [Verb("meta",
        HelpText =
            "Generate the meta template file with all applicable options.")]
    public class GenerateUploadMetaTemplateFileOptions
    {
        public const string DefaultMetaFileName = "meta.yaml";
        
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory.")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('n', "name", Required = false, HelpText = "Name of the file with extension. Defaults to 'meta.yaml'.")]
        public string FileName { get; set; } = DefaultMetaFileName;
    }
}
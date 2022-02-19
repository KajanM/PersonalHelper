using System.Collections.Generic;
using CommandLine;

namespace WindowsHelper.ConsoleOptions.Generators.Tsp
{
    [Verb("repo",
        HelpText =
            "Generate repository classes")]
    public class GenerateRepositoryOptions
    {
        [Option('n', "name", Required = false, HelpText = "Names, separate by ,")]
        public IEnumerable<string> Names { get; set; }
    }
}
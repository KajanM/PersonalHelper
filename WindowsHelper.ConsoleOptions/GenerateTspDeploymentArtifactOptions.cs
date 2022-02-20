using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("tsp-pack",
        HelpText =
            "Create deployment artifact with correct appsettings")]
   public class GenerateTspDeploymentArtifactOptions
   {
       [Option('i', "is-production", Required = false, HelpText = "Do generate production artifact")]
       public bool IsProduction { get; set; } 
   }
}
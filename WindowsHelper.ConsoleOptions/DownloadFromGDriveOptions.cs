using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("g-download",
        HelpText =
            "Fetch download links from google drive and add them to IDM")]
    public class DownloadFromGDriveOptions
    {
        [Option('p', Required = false, Default = "kajan", HelpText = "The profile name.")]
        public string Profile { get; set; } = "kajan";
    }
}
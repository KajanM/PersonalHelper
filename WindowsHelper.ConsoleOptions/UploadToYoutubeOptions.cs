using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    public enum YoutubeKeyPair
    {
        KeyPairOne,
        KeyPairTwo
    }
    
    [Verb("upload",
        HelpText =
            "Upload videos in the current directory to youtube, playlist will be created based on the directory name.")]
    public class UploadToYoutubeOptions
    {
        private bool _doesPlaylistAlreadyExist;
        
        [Option('p', "path", Required = false, HelpText = "Path to perform action. Defaults to the current directory")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option("pl-exist", Required = false, HelpText = "Does playlist already exist? Defaults to false")]
        public bool DoesPlaylistAlreadyExist {
            get
            {
                if (!string.IsNullOrWhiteSpace(PlaylistId)) return true;
                return _doesPlaylistAlreadyExist;
            }
            set => _doesPlaylistAlreadyExist = value;
        }
        
        [Option("pl-id", Required = false, HelpText = "Playlist Id. Optional.")]
        public string PlaylistId { get; set; }
        
        [Option("url", Required = false, HelpText = "The URL to use when creating Notion entry.")]
        public string Url { get; set; }
        
        [Option('s', "shutdown", Required = false, HelpText = "Shutdown once the process is completed.")]
        public bool DoShutDown { get; set; }

        [Option('k', "key", Required = false, HelpText = "Which key pair file to use. Defaults to KeyPairOne")]
        public YoutubeKeyPair KeyPairToUse { get; set; } = YoutubeKeyPair.KeyPairTwo;
    }
}
﻿using System;
using CommandLine;

namespace WindowsHelper.ConsoleOptions
{
    [Verb("upload",
        HelpText =
            "Upload videos in the current directory to youtube, playlist will be created based on the directory name.")]
    public class UploadToYoutubeOptions
    {
        private bool _doesPlaylistAlreadyExist;
        private bool _shouldAddEntryToNotion;
        
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
        
        [Option('n', "notion", Required = false, HelpText = "Should add entry to notion?")]
        public bool ShouldAddEntryToNotion
        {
            get => _shouldAddEntryToNotion || !DoesPlaylistAlreadyExist;
            set => _shouldAddEntryToNotion = value;
        }
        
        [Option('s', "shutdown", Required = false, HelpText = "Shutdown once the process is completed.")]
        public bool DoShutDown { get; set; }
    }
}
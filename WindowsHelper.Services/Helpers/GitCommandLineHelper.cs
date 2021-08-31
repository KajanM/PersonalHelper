using System.Collections.Generic;
using CliWrap.Buffered;
using Serilog;
using WindowsHelper.Services.Helpers.Http;

namespace WindowsHelper.Services.Helpers
{
    public static class GitCommandLineHelper
    {
        private const string GitExecutablePath = "git.exe";

        public static BufferedCommandResult TrackFiles(string repositoryPath,
            params string[] filesToTrack)
        {
            Log.Information("Tracking changes to git. Files {Files}", filesToTrack);

            var arguments = new List<string> { "add" };
            arguments.AddRange(filesToTrack);

            return CliWrapperHelper.Execute(new CliWrapperOptions
            {
                ExecutablePath = GitExecutablePath,
                Arguments = arguments,
            });
        }

        public static BufferedCommandResult CommitChanges(string repoPath, string message)
        {
            Log.Information("Committing changes with message {Message}", message);
            return CliWrapperHelper.Execute(new CliWrapperOptions
            {
                ExecutablePath = GitExecutablePath,
                Arguments = new[] { "commit", "-m", message },
                WorkingDirectory = repoPath
            });
        }

        public static BufferedCommandResult PushChanges(string repoPath,
            string remoteName = "origin", string branchName = "master")
        {
            Log.Information("Pushing changes to {RemoteName} {BranchName}", remoteName, branchName);

            return CliWrapperHelper.Execute(new CliWrapperOptions
            {
                ExecutablePath = GitExecutablePath,
                Arguments = new[] { "push", remoteName, branchName },
                WorkingDirectory = repoPath
            });
        }
    }
}
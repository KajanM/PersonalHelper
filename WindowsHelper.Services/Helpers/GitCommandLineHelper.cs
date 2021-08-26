using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Serilog;

namespace WindowsHelper.Services.Helpers
{
    public class GitCommandLineHelper
    {
        public static async Task<BufferedCommandResult> TrackFilesAsync(string repositoryPath,
            params string[] filesToTrack)
        {
            Log.Information("Tracking changes to git. Files {Files}", filesToTrack);
            
            var arguments = new List<string> { "add" };
            arguments.AddRange(filesToTrack);

            var commandResult = await Cli.Wrap("git.exe")
                .WithArguments(arguments)
                .WithWorkingDirectory(repositoryPath)
                .ExecuteBufferedAsync();

            Log.Information(commandResult.StandardOutput);
            Log.Error(commandResult.StandardError);

            return commandResult;
        }

        public static async Task<BufferedCommandResult> CommitChangesAsync(string repoPath, string message)
        {
            Log.Information("Committing changes with message {Message}", message);
            var commandResult = await Cli.Wrap("git.exe")
                .WithArguments(new[] { "commit", "-m", message })
                .WithWorkingDirectory(repoPath)
                .ExecuteBufferedAsync();

            Log.Information(commandResult.StandardOutput);
            Log.Error(commandResult.StandardError);

            return commandResult;
        }

        public static async Task<BufferedCommandResult> PushChangesAsync(string repoPath,
            string remoteName = "origin", string branchName = "master")
        {
            Log.Information("Pushing changes to {RemoteName} {BranchName}", remoteName, branchName);
            var commandResult = await Cli.Wrap("git.exe")
                .WithArguments(new[] { "push", remoteName, branchName })
                .WithWorkingDirectory(repoPath)
                .ExecuteBufferedAsync();

            Log.Information(commandResult.StandardOutput);
            Log.Error(commandResult.StandardError);

            return commandResult;
        }
    }
}
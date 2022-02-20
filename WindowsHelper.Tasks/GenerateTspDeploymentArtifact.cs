using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using LibGit2Sharp;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Shared;

namespace WindowsHelper.Tasks
{
    public static class GenerateTspDeploymentArtifact
    {
        public static void Execute(GenerateTspDeploymentArtifactOptions options, TspProjectSettings projectSettings)
        {
            var repoPath = Path.Join(projectSettings.ProjectRoot, "..");
            var buildDirectoryPath = Path.Join(projectSettings.ProjectRoot, "bin", "Debug", "netcoreapp3.1", "publish");
            if (!Directory.Exists(buildDirectoryPath))
            {
                throw new ApplicationException($"{buildDirectoryPath} does not exist");
            }

            var artifactName = GetArtifactName(repoPath);
            CopyFilesFromSourceToTargetDirectory(
                options.IsProduction ? projectSettings.ProdSettingsPath : projectSettings.UatSettingsPath,
                buildDirectoryPath);
            GenerateZip(buildDirectoryPath, projectSettings.DeploymentArtifactDirectory, artifactName);
        }

        private static string GetArtifactName(string repoPath)
        {
            var commitSha = GetLastCommitSha(repoPath);
            var now = DateTime.Now.AddHours(2).AddMinutes(30); // SGT zone

            return $"{now:yyyyMMdd}_{now:HHmm}_{commitSha}.zip";
        }

        private static void GenerateZip(string buildDirectoryPath,
            string deploymentArtifactDirectory, string artifactName)
        {
            var artifactPath = Path.Join(deploymentArtifactDirectory, artifactName);
            Log.Information("Generating artifact at {Path}", artifactPath);
            try
            {
                ZipFile.CreateFromDirectory(buildDirectoryPath, artifactPath);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while generating artifact at {Destination}", artifactPath);
                throw;
            }
        }

        private static void CopyFilesFromSourceToTargetDirectory(string source, string target)
        {
            var sourceDirectory = new DirectoryInfo(source);
            if (!sourceDirectory.Exists)
            {
                throw new ApplicationException($"{source} does not exist");
            }

            foreach (var file in sourceDirectory.GetFiles())
            {
                var targetPath = Path.Join(target, $"{file.Name}");
                Log.Information("Copying {Source} to {Target}", file.FullName, targetPath);
                try
                {
                    file.CopyTo(targetPath, true);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while copying {Source} to {Target}", file.FullName, targetPath);
                    throw;
                }
            }
        }

        private static string GetLastCommitSha(string repoPath)
        {
            using var repo = new Repository(repoPath);
            var lastCommit = repo.Commits.OrderByDescending(c => c.Author.When.Date).FirstOrDefault();
            if (lastCommit == null)
            {
                throw new ApplicationException("Unable to get the commit SHA");
            }

            return lastCommit.Sha[0..8];
        }
    }
}
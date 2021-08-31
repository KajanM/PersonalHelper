using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Helpers;

namespace WindowsHelper.Tasks
{
    public class UpdateFreshCouponsRepo
    {
        public static async Task ExecuteAsync(UpdateFreshCouponsRepoOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            options.FileName ??= $"-{DateTime.Now:dd-MM-yyyy}.json";
            var coursesJsonFiles = directory.GetFiles().Where(file => file.Name.StartsWith("udemy-") && file.Name.Contains(options.FileName)).ToList();
            
            if (!coursesJsonFiles.Any())
            {
                Log.Information("No file found with the name patter {Pattern}", options.FileName);
                return;
            }
            
            if (coursesJsonFiles.Count > 1)
            {
                Log.Information("Found {Count} files with the pattern {Pattern}", coursesJsonFiles.Count, options.FileName);
                return;
            }

            var fileToMove = coursesJsonFiles.First();
            MoveFileToRepoDirectory(options, fileToMove);
            await UpdateMetaFileAsync(options, fileToMove.FullName);
            AddChangesToGit(options, fileToMove.Name);
            if(options.IsDryRun) return;
            GitCommandLineHelper.CommitChanges(options.RepoPath, $"add new coupons: {fileToMove.Name}");
            GitCommandLineHelper.PushChanges(options.RepoPath);
        }

        private static void AddChangesToGit(UpdateFreshCouponsRepoOptions options, string fileName)
        {
            GitCommandLineHelper.TrackFiles(options.RepoPath, fileName, "meta.json");
        }

        private static async Task UpdateMetaFileAsync(UpdateFreshCouponsRepoOptions options, string coursesJsonFullPath)
        {
            var metaFilePath = Path.Join(options.RepoPath, "meta.json");
            if (!File.Exists(metaFilePath))
            {
                throw new ArgumentException($"Unable to locate meta file at {metaFilePath}");
            }

            var lastSyncedTime = Path.GetFileNameWithoutExtension(coursesJsonFullPath).Split("udemy-")[1];

            var newMetaData = new MetaJson
            {
                LastSynced = lastSyncedTime
            }.SerializeObject();
                
            Log.Information("Writing {Content} to {Path}", newMetaData, metaFilePath);
            if(options.IsDryRun) return;
            await File.WriteAllTextAsync(metaFilePath, newMetaData);
        }

        private static void MoveFileToRepoDirectory(UpdateFreshCouponsRepoOptions options, FileInfo fileToMove)
        {
            var moveToPath = Path.Join(options.RepoPath, fileToMove.Name);
            if (File.Exists(moveToPath))
            {
                Log.Information("The file already exists at {MoveToPath}", moveToPath);
                return;
            }

            Log.Information("Moving {FileName} to {MoveToPath}", fileToMove.FullName, moveToPath);
            if (options.IsDryRun) return;
            File.Move(fileToMove.FullName, moveToPath);
        }

        class MetaJson
        {
            public string LastSynced { get; set; }
        }
    }
}
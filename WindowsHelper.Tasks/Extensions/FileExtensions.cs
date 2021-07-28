using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsHelper.Tasks.Helpers;
using CliWrap.Buffered;
using Serilog;

namespace WindowsHelper.Tasks.Extensions
{
    public static class FileExtensions
    {
        public static void RenameByReplacingSpecialChars(this FileInfo file, bool isDryRun, string replaceByChar = "-")
        {
            var nameExcludingTheIgnoredChars = file.Name.ReplaceInvalidChars();

            var newName = file.FullName.Replace(file.Name, $"{nameExcludingTheIgnoredChars}{file.Extension}");

            if (newName == file.FullName)
            {
                Log.Information("____Ignore");
                return;
            }

            Log.Information($"From: {file.FullName}{Environment.NewLine}To: {newName}{Environment.NewLine}");
            if (isDryRun) return;

            File.Move(file.FullName, newName);
        }

        public static async Task<(int seconds, BufferedCommandResult cmdResult)> GetMediaDurationAsync(this FileInfo file)
        {
            return await FfmpegCommandHelper.GetMediaDurationAsync(file.FullName);
        }

        public static IEnumerable<FileInfo> GetVideos(this DirectoryInfo directory)
        {
            var videoExtensions =
                new List<string>
                    {
                        "mp4", "mov", "wmv", "ts", "avi", "webm"
                    }
                    .Select(e => $".{e}").ToList();
            
            return directory.GetFiles()
                .Where(file => videoExtensions.Contains(Path.GetExtension(file.Name)))
                .AsEnumerable();
        }
    }
}
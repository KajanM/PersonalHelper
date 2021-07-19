using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsHelper.Tasks.Helpers;
using CliWrap.Buffered;

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
                Console.WriteLine("____Ignore");
                return;
            }

            Console.WriteLine($"From: {file.FullName}{Environment.NewLine}To: {newName}{Environment.NewLine}");
            if (isDryRun) return;

            File.Move(file.FullName, newName);
        }

        public static async Task<(int seconds, BufferedCommandResult cmdResult)> GetMediaDurationAsync(this FileInfo file)
        {
            return await FfmpegCommandHelper.GetMediaDurationAsync(file.FullName);
        }
    }
}
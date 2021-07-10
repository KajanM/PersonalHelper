using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsHelper.Tasks.Extensions
{
    public static class FileExtensions
    {
        public static void RenameByReplacingSpecialChars(this FileInfo file, bool isDryRun, string replaceByChar = "-")
        {
            var nameExcludingTheIgnoredChars = Regex
                .Replace(Path.GetFileNameWithoutExtension(file.Name), @"[^A-Za-z0-9]+", replaceByChar)
                .ToLower();

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
    }
}
using System;
using System.IO;
using System.Text.RegularExpressions;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public class ReplaceInvalidCharsFromFileName
    {
        private readonly ReplaceInvalidCharsFromFileNameOptions _options;

        public ReplaceInvalidCharsFromFileName(ReplaceInvalidCharsFromFileNameOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
        }

        public void Replace()
        {
            var directory = new DirectoryInfo(_options.Path);
            if (!directory.Exists) throw new ArgumentException("Invalid path");

            foreach (var file in directory.GetFiles())
            {
                var nameExcludingTheIgnoredChars = Regex
                    .Replace(Path.GetFileNameWithoutExtension(file.Name), @"[^A-Za-z0-9]+", "-")
                    .ToLower();

                var newName = file.FullName.Replace(file.Name, $"{nameExcludingTheIgnoredChars}{file.Extension}");
                if (newName == file.FullName)
                {
                    Console.WriteLine("____Ignore");
                    continue;
                }


                Console.WriteLine($"From: {file.FullName}{Environment.NewLine}To: {newName}{Environment.NewLine}");
                if (_options.IsDryRun) continue;

                File.Move(file.FullName, newName);
            }
        }
    }
}
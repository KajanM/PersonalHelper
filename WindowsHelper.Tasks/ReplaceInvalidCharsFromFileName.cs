using System;
using System.IO;
using System.Text.RegularExpressions;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public class ReplaceInvalidCharsFromFileName
    {
        private readonly ReplaceInvalidCharsFromFileNameOptions _options;
        private static readonly char[] CharsToReplace = {'.', ',', ' ', '-'};

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
                var nameWithoutExtension = file.Name.Split(file.Extension)[0];
                var nameExcludingTheIgnoredChars = string.Join("_", nameWithoutExtension.Split(CharsToReplace));
                nameExcludingTheIgnoredChars = Regex.Replace(nameExcludingTheIgnoredChars, @"_+", "_").ToLower();

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
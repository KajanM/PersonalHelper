using System;
using System.IO;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;

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
                file.RenameByReplacingSpecialChars(_options.IsDryRun);
            }
        }
    }
}
using System;
using System.IO;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Tasks.Extensions;

namespace WindowsHelper.Tasks
{
    public static class ReplaceInvalidCharsFromFileName
    {
        public static void Execute(ReplaceInvalidCharsFromFileNameOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            if (!directory.Exists) throw new ArgumentException("Invalid path");

            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                file.RenameByReplacingSpecialChars(options.IsDryRun, !options.DoNotChangeToLower);
            }
            
            foreach (var subDirectory in directory.GetDirectories("*", SearchOption.AllDirectories))
            {
               subDirectory.RenameByReplacingSpecialChars(options.IsDryRun);
            }
        }
    }
}
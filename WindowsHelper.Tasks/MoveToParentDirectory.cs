using System;
using System.IO;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public class MoveToParentDirectory
    {
        private readonly MoveToParentDirectoryOptions _options;

        public MoveToParentDirectory(MoveToParentDirectoryOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
        }

        public bool Move()
        {
            var directory = new DirectoryInfo(_options.Path);
            MoveFilesInDirectory(directory);

            return true;
        }

        private void MoveFilesInDirectory(DirectoryInfo directory)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                foreach (var file in subDirectory.GetFiles())
                {
                    var moveToPath = Path.Join(_options.Path,
                        string.Join("_", file.Name.Split(Path.GetInvalidFileNameChars())));

                    if (_options.IsDryRun)
                    {
                        Console.WriteLine(
                            $"To: {moveToPath}{Environment.NewLine}From: {file.FullName}{Environment.NewLine}");
                    }
                    else
                    {
                        if (_options.IsCopy)
                        {
                            File.Copy(file.FullName, moveToPath);
                        }
                        else
                        {
                            File.Move(file.FullName, moveToPath);
                        }
                    }
                }

                foreach (var subSubDirectory in subDirectory.GetDirectories())
                {
                    MoveFilesInDirectory(subSubDirectory);
                }
            }
        }
    }
}
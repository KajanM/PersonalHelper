using System;
using System.IO;
using WindowsHelper.ConsoleOptions;
using Serilog;
using WindowsHelper.Tasks.Extensions;

namespace WindowsHelper.Tasks
{
    public class MoveToParentDirectory
    {
        private readonly MoveToParentDirectoryOptions _options;
        private int directoryNumber = 0;

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
                directoryNumber += 1;
                var files = _options.DoMoveOnlyVideos ? subDirectory.GetVideos() : subDirectory.GetFiles();
                foreach (var file in files)
                {
                    var moveToPath = Path.Join(_options.Path,
                        $"{directoryNumber:00}-{string.Join("_", file.Name.Split(Path.GetInvalidFileNameChars()))}");

                    if (_options.IsDryRun)
                    {
                        Log.Information(
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
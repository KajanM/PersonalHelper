using System;
using System.IO;
using System.Linq;
using WindowsHelper.ConsoleOptions;

namespace FileHelper
{
    public class FileOrganizer
    {

        private readonly AppendNumberToFilesOptions _options;
        public FileOrganizer(AppendNumberToFilesOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
            
            if(!Directory.Exists(options.Path)) throw new ArgumentException($"Directory does not exist: {options.Path}");
        }

        public void PrependFileNamesWithNumber()
        {
            PrependFileNamesWithNumber(new DirectoryInfo(_options.Path), true);
        }

        private void PrependFileNamesWithNumber(DirectoryInfo directory, bool isFirstRun = false)
        {
            var isAbleToGetPrependNo = int.TryParse(directory.Name.Split("-")[0], out var directoryNumber);

            if (!isFirstRun && !isAbleToGetPrependNo)
            {
                throw new ApplicationException("Directory not in the expected format");
            }

            foreach (var file in directory.GetFiles().OrderBy(f => f.Name))
            {
                var destinationFileName = file.FullName.Replace(file.Name, $"{directoryNumber}-{file.Name}");
                if (_options.IsDryRun)
                {
                    Console.WriteLine(file.FullName);
                    Console.WriteLine(destinationFileName);
                    Console.WriteLine("****************");
                }
                else
                {
                    File.Move(file.FullName, destinationFileName);
                }
            }

            foreach (var subDirectory in directory.GetDirectories().OrderBy(d => d.Name))
            {
                PrependFileNamesWithNumber(subDirectory);
            }
        }
    }
}
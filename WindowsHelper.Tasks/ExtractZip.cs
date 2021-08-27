using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Tasks
{
    public static class ExtractZip
    {
        public static void Execute(ExtractZipOptions options)
        {
            ProcessDirectory(options.Path);
        }

        private static void ProcessDirectory(string directoryPath)
        {
            ProcessDirectory(new DirectoryInfo(directoryPath));
        }
        
        private static void ProcessDirectory(DirectoryInfo directory)
        {
            Log.Information("Processing {DirectoryPath}", directory.FullName);
            var zipFiles = directory.GetFiles().Where(file => file.Extension == ".zip").ToList();
            Log.Information("Found {Count} zip file(s)", zipFiles.Count);
            
            foreach (var zipFile in zipFiles)
            {
                var destinationPath = Path.Join(zipFile.DirectoryName, Path.GetFileNameWithoutExtension(zipFile.Name).ReplaceInvalidChars());
                Log.Information("Extracting {SourceFile} to {DestinationPath}", zipFile.FullName, destinationPath);
                try
                {
                    ZipFile.ExtractToDirectory(zipFile.FullName, destinationPath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unable to extract {SourceFile}", zipFile.FullName);
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
               ProcessDirectory(subDirectory); 
            }
        }
    }
}
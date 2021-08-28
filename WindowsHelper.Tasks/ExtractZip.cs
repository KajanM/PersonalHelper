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
            var directory = new DirectoryInfo(options.Path);
            
            Log.Information("Processing {DirectoryPath}", directory.FullName);
            var zipFiles = directory.GetFiles(".zip", SearchOption.AllDirectories).ToList();
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
        }
    }
}
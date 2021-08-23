using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace WindowsHelper.Services.Files
{
    public class IFileService
    {
        public static async Task WriteToFileAsync(Stream stream, string fullPath)
        {
            Log.Information("Writing to {FullPath}", fullPath);

            await using var fs = File.Create(fullPath);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fs);
        }
    }
}
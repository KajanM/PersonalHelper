using System.IO;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Services.Files
{
    public class IFileService
    {
        public static async Task WriteToFileAsync(Stream stream, string path, string name, string extension = null)
        {
            name = name.Trim().ReplaceInvalidChars().Truncate(200);
            var fullPath = Path.Join(path, $"{name}.{extension}");
            Log.Information("Writing to {FullPath}", fullPath);
            
            await using var fs = File.Create(fullPath);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fs);
        }
    }
}
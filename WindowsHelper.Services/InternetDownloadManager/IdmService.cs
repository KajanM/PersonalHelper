using System.IO;
using System.Threading.Tasks;
using System.Web;
using CliWrap;
using CliWrap.Buffered;
using Serilog;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Services.Download
{
    public interface IIdmService
    {
        public static async Task<BufferedCommandResult> AddToQueueAsync(string url, string downloadDirectoryPath,
            string title = null, string extension = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = HttpUtility.UrlDecode(Path.GetFileNameWithoutExtension(url));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = Path.GetExtension(url.Split("?")[0]);
            }

            var formattedName = $"{title.ReplaceInvalidChars(" ").Trim().Truncate(200)}{extension}";
            Log.Information("Adding {Url} to the IDM download queue with name {FileName}", url, formattedName);

            var commandResult = await Cli.Wrap("idman")
                .WithArguments(new []{"/d", url, "/p", downloadDirectoryPath, "/f", formattedName, "/a"})
                .ExecuteBufferedAsync();

            return commandResult;
        }
    }
}
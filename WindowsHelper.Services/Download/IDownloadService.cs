using System.IO;
using System.Threading.Tasks;

namespace WindowsHelper.Services.Download
{
    public interface IDownloadService
    {
        Task<Stream> DownloadFileAsync(string downloadUri);
    }
}
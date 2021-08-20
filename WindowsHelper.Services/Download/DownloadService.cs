using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace WindowsHelper.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;

        public DownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<Stream> DownloadFileAsync(string downloadUri)
        {
            Log.Information("Downloading File from {DownloadUri}", downloadUri);
        
            var response = await _httpClient.GetAsync(downloadUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WindowsHelper.Services.Libgen
{
    public interface ILibgenService
    {
        Task<List<LibgenSearchResultBindingModel>> CrawlSearchResultLinks(string searchTerm, int pageNo = 1);
        Task<string> GetDownloadLink(string pageLink);
    }
}
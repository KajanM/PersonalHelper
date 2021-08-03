using System.Threading.Tasks;
using WindowsHelper.Shared.Notion.ResponseViewModel;

namespace WindowsHelper.Services.Notion
{
    public interface INotionService
    {
        Task<DescribeDatabaseResponse> DescribeCoursesDatabaseAsync();
        Task<TResponse> DescribeDatabaseAsync<TResponse>(string databaseId);
    }
}
using System.Threading.Tasks;
using WindowsHelper.Services.Notion.BindingModels;
using WindowsHelper.Shared.Notion.ResponseViewModel;

namespace WindowsHelper.Services.Notion
{
    public interface INotionService
    {
        Task<AddNewCourseResponseBindingModel> AddCourseEntryAsync(AddNewCourseRequestBindingModel requestBody);
        Task<DescribeDatabaseResponse> DescribeCoursesDatabaseAsync();
        Task<TResponse> DescribeDatabaseAsync<TResponse>(string databaseId);
        Task<TResponse> CreatePageAsync<TBody, TResponse>(TBody requestBody);
    }
}
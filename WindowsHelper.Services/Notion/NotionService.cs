using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WindowsHelper.Services.Helpers.Http;
using WindowsHelper.Services.Notion.BindingModels;
using WindowsHelper.Shared;
using WindowsHelper.Shared.Notion.ResponseViewModel;
using Serilog;

namespace WindowsHelper.Services.Notion
{
    public class NotionService : INotionService
    {
        private readonly HttpClient _client;
        private readonly NotionSettings _settings;

        private const string NotionVersion = "2021-05-13";

        public NotionService(NotionSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings?.Token))
                throw new ArgumentNullException($"Please set {nameof(NotionSettings.Token)}");

            _settings = settings;
            _client = new HttpClient();
            InitializeHttpClient();
        }

        public async Task<AddNewCourseResponseBindingModel> AddCourseEntryAsync(
            AddNewCourseRequestBindingModel requestBody)
        {
            return await CreatePageAsync<AddNewCourseRequestBindingModel, AddNewCourseResponseBindingModel>(requestBody);
        }
        
        public async Task<TResponse> CreatePageAsync<TBody, TResponse>(TBody requestBody)
        {
            Log.Debug("Trying to add new Notion page {@Body}", requestBody);
            var response = await _client.PostAsync("pages", new JsonContent(requestBody));
            Log.Debug("Received response to add new Notion page {@Response}", response);

            return await JsonSerializer.DeserializeAsync<TResponse>(await response.Content.ReadAsStreamAsync());
        }

        public async Task<DescribeDatabaseResponse> DescribeCoursesDatabaseAsync()
        {
            return await DescribeDatabaseAsync<DescribeDatabaseResponse>(_settings.CoursesDatabaseId);
        }

        public async Task<TResponse> DescribeDatabaseAsync<TResponse>(string databaseId)
        {
            var streamTask = _client.GetStreamAsync($"databases/{databaseId}");
            var response = await JsonSerializer.DeserializeAsync<TResponse>(await streamTask);

            return response;
        }

        private void InitializeHttpClient()
        {
            _client.BaseAddress = new Uri("https://api.notion.com/v1/");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
            _client.DefaultRequestHeaders.Add("Notion-Version", NotionVersion);
        }
    }
}
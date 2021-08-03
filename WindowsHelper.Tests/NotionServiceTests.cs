using System;
using System.Threading.Tasks;
using WindowsHelper.Services.Notion;
using WindowsHelper.Services.Notion.BindingModels;
using WindowsHelper.Shared;
using Xunit;
using Xunit.Abstractions;

namespace WindowsHelper.Tests
{
    public class NotionServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly NotionSettings _notionSettings;
        private readonly INotionService _notionService;

        public NotionServiceTests(ITestOutputHelper output)
        {
            _output = output;
            _notionSettings = TestHelper.GetFromAppSettings<NotionSettings>("Notion");
            _notionService = new NotionService(new NotionSettings
            {
                CoursesDatabaseId = _notionSettings.CoursesDatabaseId,
                Token = _notionSettings.Token
            });
        }

        [Fact]
        public async Task AddCourseEntryAsync_ShouldSucceed_InHappyPath()
        {
            #region Arrange

            var requestBody = new AddNewCourseRequestBindingModel(
                _notionSettings.CoursesDatabaseId,
                "https://kajanm.com/",
                "https://kajanm.com/",
                $"test-add-page-{DateTime.Now}"
                );

            #endregion

            #region Act

            var response = await _notionService.AddCourseEntryAsync(requestBody);

            #endregion

            #region Assert

            Assert.NotEmpty(response?.Url);

            #endregion
        }

        [Fact]
        public async Task DescribeCoursesDatabaseAsync_ShouldSucceed_InHappyPath()
        {
            #region Act

            var coursesDatabase = await _notionService.DescribeCoursesDatabaseAsync();

            #endregion

            #region Assert

            Assert.Equal("database", coursesDatabase?.Object);

            #endregion
        }
    }
}
using System.Threading.Tasks;
using WindowsHelper.Services.Notion;
using WindowsHelper.Shared;
using Xunit;
using Xunit.Abstractions;

namespace WindowsHelper.Tests
{
    public class NotionServiceTests
    {
        private readonly ITestOutputHelper _output;

        [Fact]
        public async Task DescribeCoursesDatabaseAsync_ShouldSucceed_InHappyPath()
        {
            #region Arrange

            var notionSettings = TestHelper.GetFromAppSettings<NotionSettings>("Notion");

            INotionService service = new NotionService(new NotionSettings
            {
                CoursesDatabaseId = notionSettings.CoursesDatabaseId,
                Token = notionSettings.Token
            });

            #endregion

            #region Act

            var coursesDatabase = await service.DescribeCoursesDatabaseAsync();

            #endregion

            #region Assert

            Assert.Equal("database", coursesDatabase?.Object);

            #endregion
        }
    }
}
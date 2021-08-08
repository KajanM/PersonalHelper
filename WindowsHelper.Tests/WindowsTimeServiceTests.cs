using WindowsHelper.ConsoleApp;
using Xunit;

namespace WindowsHelper.Tests
{
    public class WindowsTimeServiceTests
    {
        [Fact]
        public void Should_Be_Able_To_Fetch_Time_From_Remote_Api()
        {
            var timeApiResponse = WindowsTimeService.GetUtcTimeFromApi().Result;

            Assert.NotEqual(default, timeApiResponse.DateTime);
            Assert.Equal("Etc/UTC", timeApiResponse.Timezone);
        }
    }
}
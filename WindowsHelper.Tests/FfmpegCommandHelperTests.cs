using System;
using System.Threading.Tasks;
using WindowsHelper.Tasks.Helpers;
using CliWrap;
using CliWrap.Buffered;
using Xunit;
using Xunit.Abstractions;

namespace WindowsHelper.Tests
{
    public class FfmpegCommandHelperTests
    {
        private readonly ITestOutputHelper _output;

        public FfmpegCommandHelperTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetMediaDurationAsync_ShouldReturnCorrectDuration_InHappyPath()
        {
            var (seconds, cmdResult) = await FfmpegCommandHelper.GetMediaDurationAsync("resources/dont-worry.mp4");
            Assert.Equal(207, seconds);
            Assert.Empty(cmdResult.StandardError);
        }
    }
}
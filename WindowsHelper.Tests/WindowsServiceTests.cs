using WindowsHelper.Services.Windows;
using Xunit;

namespace WindowsHelper.Tests
{
    public class WindowsServiceTests
    {
        [Fact]
        public void Shutdown_ShouldShutdownTheComputer()
        {
          WindowsService.Shutdown(3);  
        } 
    }
}
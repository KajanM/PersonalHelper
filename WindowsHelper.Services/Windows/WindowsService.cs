using System.Diagnostics;

namespace WindowsHelper.Services.Windows
{
    public static class WindowsService
    {
        public static void Shutdown(int minutes, int hours = 0, int seconds = 0)
        {
            var totalSeconds = hours * 60 * 60 + minutes * 60 + seconds;
            
            Process.Start("shutdown", $"/s /t {totalSeconds}");
        }
    }
}
using System.Diagnostics;

namespace WindowsHelper.Services.Windows
{
    public static class WindowsService
    {
        public static void Shutdown(int minutes = 0)
        {
            Process.Start("shutdown", $"/s /t {minutes * 60}");
        }
    }
}
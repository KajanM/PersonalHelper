using System.Diagnostics;

namespace WindowsHelper.Services.Windows
{
    public static class WindowsService
    {
        public static void Shutdown(int hours = 0)
        {
            Process.Start("shutdown", $"/s /t {hours * 3600}");
        }
    }
}
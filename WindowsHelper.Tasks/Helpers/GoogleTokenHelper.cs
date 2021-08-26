using System.Diagnostics;
using System.IO;

namespace WindowsHelper.Tasks.Helpers
{
    public static class GoogleTokenHelper
    {
        public static string GetTokenDirectoryPath(string profile, int credentialIndex)
        {
            return Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "yt", profile,
                $"token-{credentialIndex}");
        }
    }
}
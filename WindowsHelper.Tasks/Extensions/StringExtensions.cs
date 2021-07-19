using System.IO;
using System.Text.RegularExpressions;

namespace WindowsHelper.Tasks.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceInvalidChars(this string @this, string replaceByChar = "-")
        {
            return Regex
                .Replace(Path.GetFileNameWithoutExtension(@this), @"[^A-Za-z0-9]+", replaceByChar)
                .ToLower();
        }
    }
}
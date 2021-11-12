using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsHelper.Services.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceInvalidChars(this string @this, bool doChangeToLower = true, string replaceByChar = "-", bool isWithoutExtension = true)
        {
            var name = isWithoutExtension ? @this : Path.GetFileNameWithoutExtension(@this);
            var replaced = Regex
                .Replace(name, @"[^A-Za-z0-9]+", replaceByChar);
            if (doChangeToLower)
            {
                replaced = replaced.ToLower();
            }
            
            return $"{replaced[^1]}" == replaceByChar ? replaced[..^1] : replaced;
        }
        /// <summary>
        /// Truncates string so that it is no longer than the specified number of characters.
        /// </summary>
        /// <param name="str">String to truncate.</param>
        /// <param name="length">Maximum string length.</param>
        /// <returns>Original string or a truncated one if the original was too long.</returns>
        public static string Truncate(this string str, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be >= 0");
            }

            if (str == null)
            {
                return null;
            }

            var maxLength = Math.Min(str.Length, length);
            return str[..maxLength];
        }
    }
}
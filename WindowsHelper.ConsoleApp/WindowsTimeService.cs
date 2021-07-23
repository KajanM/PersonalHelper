using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using Serilog;

namespace WindowsHelper.ConsoleApp
{
    public static class WindowsTimeService
    {
        private const string UtcTimeApi = "https://worldtimeapi.org/api/timezone/etc/utc";
        
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear { get; set; }
            public short wMonth { get; set; }
            public short wDayOfWeek { get; set; }
            public short wDay { get; set; }
            public short wHour { get; set; }
            public short wMinute { get; set; }
            public short wSecond { get; set; }
            public short wMilliseconds { get; set; }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        public static async Task<TimeApiResponse> GetUtcTimeFromApi()
        {
            using var httpClient = new HttpClient();
            var stream = await httpClient.GetStreamAsync(UtcTimeApi);

            return await JsonSerializer.DeserializeAsync<TimeApiResponse>(stream);
        }

        public async static Task<bool> ResetTime(ChangeSystemTimeOptions options)
        {
            Log.Information($"Fetching UTC time from {UtcTimeApi}");
            var response = await GetUtcTimeFromApi();

            var currentTime = response.DateTime;
            Log.Information($"Latest time information received {currentTime}");

            options.Year = (short)currentTime.Year;
            options.Month = (short)currentTime.Month;
            options.Day = (short)currentTime.Day;
            options.Hour = (short)currentTime.Hour;
            options.Minute = (short)currentTime.Minute;
            options.Second = (short)currentTime.Second;
            options.Millisecond = (short)currentTime.Millisecond;

            return ChangeTime(options);
        }

        public static bool ChangeTime(ChangeSystemTimeOptions options)
        {
            var utcNow = DateTime.UtcNow;

            var toDateTimeLocal = new DateTime(
                options.Year ?? (short) utcNow.Year,
                options.Month ?? (short) utcNow.Month,
                options.Day ?? (short) utcNow.Day,
                options.Hour ?? (short) utcNow.Hour,
                options.Minute ?? (short) utcNow.Minute,
                options.Second ?? (short) utcNow.Second,
                options.Millisecond ?? (short) utcNow.Millisecond
            );

            var toDateTimeUtc = toDateTimeLocal.AddHours(-5).AddMinutes(-30);

            if (!options.IsDryRun)
            {
                var st = new SYSTEMTIME
                {
                    wYear = (short) toDateTimeUtc.Year,
                    wMonth = (short) toDateTimeUtc.Month,
                    wDayOfWeek = (short) toDateTimeUtc.DayOfWeek,
                    wDay = (short) toDateTimeUtc.Day,
                    wHour = (short) toDateTimeUtc.Hour,
                    wMinute = (short) toDateTimeUtc.Minute,
                    wSecond = (short) toDateTimeUtc.Second,
                    wMilliseconds = (short) toDateTimeUtc.Millisecond
                };

                Log.Information($"Current time is {utcNow.UtcToSriLankaTime()}");
                Log.Information($"Changing system time to {toDateTimeLocal}");
                
                var isSuccess = SetSystemTime(ref st);

                if (!isSuccess)
                {
                    Log.Error("Unable to change time, are you running with admin privilege?");
                    return false;
                }

                Log.Information($"Current time is {DateTime.UtcNow.UtcToSriLankaTime()}");
            }
            else
            {
                Log.Information($"Current time is {utcNow}");
                Log.Information($"The time will be set to {toDateTimeLocal}");
            }

            return true;
        }

        public static DateTime UtcToSriLankaTime(this DateTime @this)
        {
            return @this.AddHours(5).AddMinutes(30);
        }
    }

    public class TimeApiResponse
    {
        [JsonPropertyName("datetime")]
        public DateTime DateTime { get; set; }
        
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Services.Helpers
{
    public static class Utils
    {
        public static async Task<string> GetScriptAsync(string prefix, string fileName)
        {
            var scriptPath = Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                prefix, "JsScripts", fileName);
            return await File.ReadAllTextAsync(scriptPath);
        }

        public static string SerializeObject<T>(T source)
        {
            return JsonConvert.SerializeObject(source, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }

        public static float ElapsedInMinutes(this Stopwatch stopwatch)
        {
            return stopwatch.ElapsedMilliseconds / 60000;
        }

        public static async Task WriteToYamAsync<T>(string filePathWithName, T source) where T : class
        {
            try
            {
                var content = source.SerializeToYaml();
                await File.WriteAllTextAsync($"{filePathWithName}-{DateTime.Now:hh-mm-dd-MM-yyyy}.yml", content);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while writing to {FileName}.yml. Data: {@Data}",
                    filePathWithName, source);
            }
        }

        public static string GetFormattedFileName(string title, string extension)
        {
            title = title.Trim().ReplaceInvalidChars().Truncate(200);
            return $"{title}.{extension}";
        }
    }
}
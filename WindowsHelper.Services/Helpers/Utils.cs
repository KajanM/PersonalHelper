using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
    }
}
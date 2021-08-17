using System.IO;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Tasks
{
    public static class GenerateUploadMetaTemplateFile
    {
        public static async Task ExecuteAsync(GenerateUploadMetaTemplateFileOptions options)
        {
            await File.WriteAllTextAsync(Path.Join(options.Path, options.FileName),
                (new UploadToYoutubeOptions()).SerializeToYaml());
        }
    }
}
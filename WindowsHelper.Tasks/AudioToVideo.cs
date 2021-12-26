using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FFMpegCore;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Tasks
{
    public class AudioToVideo
    {
        public static void Execute(AudioToVideoOptions options)
        {
            var directory = new DirectoryInfo(options.Path);
            var audiosToJoin = directory.GetFiles().Where(f => f.Extension == $".{options.Extension}")
                .ToArray();

            var posterImagePath = Path.Join(Path.GetDirectoryName(Process.GetCurrentProcess()?.MainModule?.FileName),
                "resources", "icon.png");
            
            Log.Information("Found {Count} audios", audiosToJoin.Length);

            foreach (var audio in audiosToJoin)
            {
                var outputFullPath = Path.Join(options.Path, $"{Path.GetFileNameWithoutExtension(audio.Name).ReplaceInvalidChars()}.mp4");
                Log.Information("Converting {Source} to {Destination}", audio.Name, outputFullPath);
                try
                {
                    FFMpeg.PosterWithAudio(posterImagePath, audio.FullName, outputFullPath);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while converting {Source} to {Destination}", audio.Name,
                        outputFullPath);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services;
using WindowsHelper.Services.Download;
using WindowsHelper.Services.Extensions;

namespace WindowsHelper.Tasks
{
    public static class AddToIdmQueue
    {
        private const string DownloadSubDirectory = "downloads";

        public static async Task ExecuteAsync(AddToIdmQueueOptions options)
        {
            Log.Information("Starting to add to IDM queue. {@Options}", options);
            var directory = new DirectoryInfo(options.Path);

            var downloadDirectoryPath = Path.Join(options.Path, DownloadSubDirectory);
            if (!Directory.Exists(downloadDirectoryPath))
            {
                Directory.CreateDirectory(downloadDirectoryPath);
            }

            var tasks = new List<Task>();
            List<ToIdmQueue> links = null;
            var downloadLists = directory.GetFiles()
                .Where(file => file.Extension == ".yml" && file.Name.StartsWith("idm--")).ToList();

            Log.Information("Found {Count} download descriptors", downloadLists.Count);

            foreach (var downloadList in downloadLists)
            {
                var content = File.ReadAllTextAsync(downloadList.FullName).Result;
                try
                {
                    links = content.DeserializeYaml<List<ToIdmQueue>>();
                    Log.Information("Links to add {@Links}", links);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while deserializing {FileName}", downloadList.FullName);
                }

                if (links == null || !links.Any())
                {
                    Log.Information("No links found in {FileName}", downloadList.FullName);
                }

                links.ForEach(link =>
                {
                    tasks.Add(IIdmService.AddToQueueAsync(link.DownloadUri, downloadDirectoryPath, link.Title,
                        $".{link.Extension}"));
                });

                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while adding to IDM queue");
                }
            }
        }
    }
}
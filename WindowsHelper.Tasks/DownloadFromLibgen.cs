using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PuppeteerSharp;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Download;
using WindowsHelper.Services.Files;
using WindowsHelper.Services.Helpers;
using WindowsHelper.Services.Libgen;

namespace WindowsHelper.Tasks
{
    public class DownloadFromLibgen
    {
        private readonly ILibgenService _libgenService;
        private readonly Browser _browser;
        private readonly IDownloadService _downloadService;

        public DownloadFromLibgen()
        {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            _browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Timeout = 60000
            }).Result;
            _libgenService = new LibgenService(_browser);
            _downloadService = new DownloadService(new HttpClient());
        }

        public async Task ExecuteAsync(DownloadFromLibgenOptions options)
        {
            Log.Information("Starting to download from libgen. {@Options}", options);
            for (var i = 0; i < options.PageCountToDownload; i++)
            {
                var results = new List<LibgenSearchResultBindingModel>();
                var currentPageNo = options.StartPageNumber + i;

                try
                {
                    results.AddRange(await _libgenService.CrawlSearchResultLinks(options.SearchTerm, currentPageNo));
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while getting results from {PageNo}", currentPageNo);
                }

                foreach (var result in results)
                {
                    var filePath = Path.Join(options.Path, Utils.GetFormattedFileName(result.Title, result.Extension));
                    
                    // ignore if the file is already downloaded
                    if (File.Exists(filePath))
                    {
                        Log.Information("Skipping {FullPath} since a file already exists", filePath);
                        continue;
                    }
                    
                    foreach (var mirror in result.Mirrors)
                    {
                        try
                        {
                            var downloadUri = await _libgenService.GetDownloadLink(mirror);
                            result.DownloadUris.Add(downloadUri);
                            if (!options.DoDownload) continue;

                            try
                            {
                                var ms = await _downloadService.DownloadFileAsync(downloadUri);
                                await IFileService.WriteToFileAsync(ms, filePath);
                                break;
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Unable to download from {Uri}", downloadUri);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "An error occured while getting download link for {@BookData}", result);
                        }
                    }
                }

                await Utils.WriteToYamAsync($"libgen-{currentPageNo}", results);
            }
        }
    }
}
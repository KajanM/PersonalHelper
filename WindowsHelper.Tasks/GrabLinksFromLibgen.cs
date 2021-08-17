using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using Serilog;
using WindowsHelper.ConsoleOptions;
using WindowsHelper.Services.Helpers;
using WindowsHelper.Services.Libgen;

namespace WindowsHelper.Tasks
{
    public class GrabLinksFromLibgen
    {
        private readonly ILibgenService _libgenService;
        private readonly Browser _browser;

        public GrabLinksFromLibgen()
        {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            _browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Timeout = 60000
            }).Result;
            _libgenService = new LibgenService(_browser);
        }

        public async Task ExecuteAsync(GrabLinksFromLibgenOptions options)
        {
            Log.Information("Starting to crawl libgen. {@Options}", options);
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
                    try
                    {
                        result.DownloadUri = await _libgenService.GetDownloadLink(result.Mirror);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "An error occured while getting download link for {@BookData}", result);
                    }
                }

                await Utils.WriteToYamAsync($"libgen-{currentPageNo}", results);
            }
        }
    }
}
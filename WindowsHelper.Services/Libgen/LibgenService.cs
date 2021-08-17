using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using Serilog;
using WindowsHelper.Services.Helpers;

namespace WindowsHelper.Services.Libgen
{
    public class LibgenService : ILibgenService
    {
        private readonly Browser _browser;
        private readonly string _parseLibgenSearchResultsScript;

        public LibgenService(Browser browser)
        {
            _browser = browser;
            _parseLibgenSearchResultsScript = Utils.GetScriptAsync("Libgen", "ParseLibgenSearchResults.js").Result;
        }

        public async Task<List<LibgenSearchResultBindingModel>> CrawlSearchResultLinks(string searchTerm,
            int pageNo = 1)
        {
            var url = GetUrl(searchTerm, pageNo);
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(url);
            page.WaitForSelectorAsync(".c");
            var resultsCountText =
                await page.EvaluateExpressionAsync<string>(
                    "document.querySelector('table:nth-of-type(2) td font').textContent");
            Log.Information("Found {ResultsCount}", resultsCountText);
            var isValidResultsCount = int.TryParse(resultsCountText.Split(" ")[0], out var resultsCount);
            if (!isValidResultsCount || resultsCount <= 0) return null;

            var results =
                await page.EvaluateFunctionAsync<List<LibgenSearchResultBindingModel>>(_parseLibgenSearchResultsScript);

            Log.Information("Parsed {Count} links from libgen", results.Count);

            await page.CloseAsync();

            return results;
        }

        public async Task<string> GetDownloadLink(string pageLink)
        {
            var page = await _browser.NewPageAsync();

            await page.GoToAsync(pageLink);
            await page.WaitForSelectorAsync("#download");
            var downloadLink = await page.EvaluateExpressionAsync<string>("document.querySelector('#download a').href");
            Log.Information("Successfully grabbed download link for {PageLink}. Download link: {DownloadLink}", pageLink, downloadLink);

            return downloadLink;
        }

        private static string GetUrl(string searchTerm, int pageNo)
        {
            return
                $"https://libgen.is/search.php?req={Uri.EscapeDataString(searchTerm)}&lg_topic=libgen&open=0&view=simple&res=100&phrase=1&column=def&page={pageNo}";
        }
    }
}
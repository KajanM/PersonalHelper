using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using Serilog;
using WindowsHelper.Services.Helpers;

namespace WindowsHelper.Services.Udemy
{
    public class RealDiscountCrawlingService : IRealDiscountCrawlingService
    {
        private readonly Browser _browser;
        private readonly string _parseCoursesWithCouponScript;

        public RealDiscountCrawlingService(Browser browser)
        {
            _browser = browser;
            _parseCoursesWithCouponScript = Utils.GetScriptAsync("Udemy", "ParseCoursesFromRealDiscount.js").Result;
        }

        public async Task<IEnumerable<RealDiscountCourseBindingModel>> GetCoursesAsync(int numberOfPagesToCrawl = 1,
            int pageNoToStart = 1)
        {
            var courses = new List<RealDiscountCourseBindingModel>();
            for (var i = 0; i < numberOfPagesToCrawl; i++)
            {
                courses.AddRange(
                    await GetCoursesInPageAsync(pageNoToStart + i));
            }

            return courses.Distinct();
        }

        private async Task<IEnumerable<RealDiscountCourseBindingModel>> GetCoursesInPageAsync(int pageNo = 1)
        {
            var page = await FetchCourseListPage(pageNo);
            var courses = await page.EvaluateFunctionAsync<List<RealDiscountCourseBindingModel>>(
                _parseCoursesWithCouponScript);

            Log.Information("Parsed {ParsedCoursesCount} in page {PageNo}", courses.Count, pageNo);

            await page.CloseAsync();
            return courses;
        }

        public async Task<string> GetUdemyUriWithCouponAsync(string courseDetailsPageUri)
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(courseDetailsPageUri);
            await page.WaitForSelectorAsync(".container-fluid a center");
            var combinedUri = await
                page.EvaluateExpressionAsync<string>(
                    "document.querySelector('.container-fluid a center').parentElement.parentElement.href");
            await page.CloseAsync();
            var startIndexOfUdemyUri = combinedUri.IndexOf("https://www.udemy.com", StringComparison.Ordinal);
            if (startIndexOfUdemyUri < 0)
                throw new ApplicationException($"URI: {combinedUri} does not include udemy coupon.");

            return combinedUri[startIndexOfUdemyUri..];
        }

        private async Task<Page> FetchCourseListPage(int pageNo = 1)
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(GetCourseListPageUrl(pageNo));
            await page.WaitForSelectorAsync(".row.main-bg .col-sm-12.col-md-6.col-lg-4.col-xl-4");
            return page;
        }

        private static string GetCourseListPageUrl(int pageNo = 1)
        {
            return
                $"https://app.real.discount/filter/?category=All&store=Udemy&duration=All&price=0&rating=All&language=English&search=&submit=Filter&page={pageNo}";
        }
    }
}
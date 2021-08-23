using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using Serilog;
using WindowsHelper.Services.Helpers;
using WindowsHelper.Services.Udemy;

namespace WindowsHelper.Tasks.Udemy
{
    public class CrawlUdemyCoupons
    {
        private readonly Browser _browser;
        private readonly IRealDiscountCrawlingService _realDiscountService;
        private readonly IUdemyCrawlingService _udemyService;
        private readonly Dictionary<string, UdemyCourseBindingModel> _courseByUri;
        private readonly List<RealDiscountCourseBindingModel> _coursesWithParsingError;

        public CrawlUdemyCoupons()
        {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Wait();
            _browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Timeout = 60000
            }).Result;
            _realDiscountService = new RealDiscountCrawlingService(_browser);
            _udemyService = new UdemyCrawlingService(_browser);
            _courseByUri = new Dictionary<string, UdemyCourseBindingModel>();
            _coursesWithParsingError = new List<RealDiscountCourseBindingModel>();
        }

        public async Task ExecuteAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var parsedCoursesFromRealDiscount = (await _realDiscountService.GetCoursesAsync(2)).ToList();
            await WriteToJsonAsync("real_discount", parsedCoursesFromRealDiscount);
            foreach (var courseDataFromRealDiscount in parsedCoursesFromRealDiscount)
            {
                try
                {
                    var udemyUri =
                        await _realDiscountService.GetUdemyUriWithCouponAsync(courseDataFromRealDiscount.Link);
                    var uriWithoutCoupon = new Uri(udemyUri).GetLeftPart(UriPartial.Path);

                    if (_courseByUri.ContainsKey(uriWithoutCoupon)) continue;
                    
                    var courseDetails = await _udemyService.GetCourseDetailsAsync(udemyUri);

                    _courseByUri.Add(uriWithoutCoupon, courseDetails);

                    Log.Information("Udemy URI: {URI}, CourseDetails: {@Details}", udemyUri, courseDetails);
                }
                catch (Exception e)
                {
                    Log.Error(e, "An error occured while getting course details of {@Details}",
                        courseDataFromRealDiscount);
                    _coursesWithParsingError.Add(courseDataFromRealDiscount);
                }
            }

            await WriteToJsonAsync("udemy", _courseByUri);
            if (_coursesWithParsingError.Any())
            {
                await WriteToJsonAsync("errors", _coursesWithParsingError);
            }

            Log.Information("Process completed in {Elapsed} minutes.", stopwatch.ElapsedInMinutes());
            Log.Information("Found: {RealDiscountCouponCount}. Processed: {ProcessedCount}. Errors: {ErrorsCount}",
                parsedCoursesFromRealDiscount.Count,
                _courseByUri.Count, _coursesWithParsingError.Count);
        }

        private static async Task WriteToJsonAsync<T>(string filePrefix, T source)
        {
            var content = Utils.SerializeObject(source);
            await File.WriteAllTextAsync($"{filePrefix}-{DateTime.Now:hh-mm-dd-MM-yyyy}.json", content);
        }
    }
}
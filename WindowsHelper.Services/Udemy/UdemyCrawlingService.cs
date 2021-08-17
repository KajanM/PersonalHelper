using System;
using System.Threading.Tasks;
using System.Web;
using PuppeteerSharp;
using WindowsHelper.Services.Helpers;

namespace WindowsHelper.Services.Udemy
{
    public class UdemyCrawlingService : IUdemyCrawlingService
    {
        private readonly Browser _browser;
        private readonly string _parseCourseDetailsScript;

        public UdemyCrawlingService(Browser browser)
        {
            _browser = browser;
            _parseCourseDetailsScript = Utils.GetScriptAsync("Udemy", "ParseCourseDetailsFromUdemy.js").Result;
        }

        public async Task<UdemyCourseBindingModel> GetCourseDetailsAsync(string uriWithCoupon)
        {
            var uri = new Uri(uriWithCoupon);
            var coupon = HttpUtility.ParseQueryString(uri.Query).Get("couponCode");
            if (string.IsNullOrWhiteSpace(coupon))
                throw new ArgumentException($"{uriWithCoupon} does not contain the coupon code");
            
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(uriWithCoupon);
            await page.WaitForSelectorAsync("[data-purpose='discount-percentage']");
            
            var courseDetails = await page.EvaluateFunctionAsync<UdemyCourseBindingModel>(_parseCourseDetailsScript);
            courseDetails.CourseUri = uri.GetLeftPart(UriPartial.Path);
            courseDetails.CouponCode = coupon;
            
            await page.CloseAsync();
            
            return courseDetails;
        }
    }
}
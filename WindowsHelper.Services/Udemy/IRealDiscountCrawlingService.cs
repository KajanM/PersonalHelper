using System.Collections.Generic;
using System.Threading.Tasks;

namespace WindowsHelper.Services.Udemy
{
    public interface IRealDiscountCrawlingService
    {
        Task<IEnumerable<RealDiscountCourseBindingModel>> GetCoursesAsync(int numberOfPagesToCrawl = 1, int pageNoToStart = 1);
        Task<string> GetUdemyUriWithCouponAsync(string courseDetailsPageUri);
    }
}
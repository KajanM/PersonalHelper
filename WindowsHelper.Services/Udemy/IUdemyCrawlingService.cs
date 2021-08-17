using System.Threading.Tasks;

namespace WindowsHelper.Services.Udemy
{
    public interface IUdemyCrawlingService
    {
        Task<UdemyCourseBindingModel> GetCourseDetailsAsync(string uriWithCoupon);
    }
}
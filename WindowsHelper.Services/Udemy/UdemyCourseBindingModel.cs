namespace WindowsHelper.Services.Udemy
{
    public class UdemyCourseBindingModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Language { get; set; }
        public string CourseUri { get; set; }
        public string CouponCode { get; set; }
        public string ImageUri { get; set; }
        public string PriceAfterCouponIsApplied { get; set; }
        public string DiscountPercentage { get; set; }
        public string CourseDuration { get; set; }
        public string Rating { get; set; }
        public string EnrolledStudentsCount { get; set; }
        public string LastUpdated { get; set; }
        public string CourseProviderName { get; set; }
        public string CourseProviderRating { get; set; }
        public string CourseProviderUri { get; set; }

        public bool IsValidCoupon => !string.IsNullOrWhiteSpace(DiscountPercentage);
    }
}
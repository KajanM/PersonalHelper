namespace WindowsHelper.Services.Udemy
{
    public class RealDiscountCourseBindingModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string ShortDescription { get; set; }
        public string Category { get; set; }
        public string CouponPublishedTime { get; set; }
        public string ExpiresIn { get; set; }
        public bool IsEditorsChoice { get; set; }

        protected bool Equals(RealDiscountCourseBindingModel other)
        {
            return Link == other.Link;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RealDiscountCourseBindingModel)obj);
        }

        public override int GetHashCode()
        {
            return (Link != null ? Link.GetHashCode() : 0);
        }

        public static bool operator ==(RealDiscountCourseBindingModel left, RealDiscountCourseBindingModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RealDiscountCourseBindingModel left, RealDiscountCourseBindingModel right)
        {
            return !Equals(left, right);
        }
    }
}
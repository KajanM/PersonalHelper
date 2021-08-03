namespace WindowsHelper.Shared
{
    public class AppSettings
    {
        public YoutubeSettings YoutubeSettings { get; set; }
        
        public NotionSettings NotionSettings { get; set; }
    }

    public class YoutubeSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class NotionSettings
    {
        public string Token { get; set; }
        
        public string CoursesDatabaseId { get; set; }
    }
}
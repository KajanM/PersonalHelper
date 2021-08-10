using System.Collections.Generic;

namespace WindowsHelper.Shared
{
    public class AppSettings
    {
        public YoutubeSettings YoutubeSettings { get; set; }
        
        public NotionSettings NotionSettings { get; set; }

        public SentrySettings SentrySettings { get; set; }
    }

    public class YoutubeSettings
    {
        public List<YoutubeCredentials> Credentials { get; set; }
    }

    public class YoutubeCredentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class NotionSettings
    {
        public string Token { get; set; }
        
        public string CoursesDatabaseId { get; set; }
    }

    public class SentrySettings
    {
        public string Dsn { get; set; }
    }
}
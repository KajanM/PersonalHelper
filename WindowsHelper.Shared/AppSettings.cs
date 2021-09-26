using System.Collections.Generic;

namespace WindowsHelper.Shared
{
    public class AppSettings
    {
        public GoogleSettings GoogleSettings { get; set; }
        
        public NotionSettings NotionSettings { get; set; }

        public SentrySettings SentrySettings { get; set; }
    }

    public class GoogleSettings
    {
        public List<GoogleProjectCredentials> Credentials { get; set; }
    }

    public class GoogleProjectCredentials
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
namespace WindowsHelper.Shared
{
    public class AppSettings
    {
        public YoutubeSettings YoutubeSettings { get; set; } 
    }

    public class YoutubeSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
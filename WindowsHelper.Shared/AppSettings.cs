using System.Collections.Generic;

namespace WindowsHelper.Shared
{
    public class AppSettings
    {
        public GoogleSettings GoogleSettings { get; set; }
        
        public NotionSettings NotionSettings { get; set; }

        public SentrySettings SentrySettings { get; set; }
        
        public TspProjectSettings TspProjectSettings { get; set; } 
    }

    public class GoogleSettings
    {
        public List<GoogleProjectCredentials> Credentials { get; set; }
    }

    public class GoogleProjectCredentials
    {
        public string Mail { get; set; }
        public string ProjectId { get; set; }
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

    public class TspProjectSettings
    {
        public string ProjectRoot { get; set; }
        
        public string UatSettingsPath { get; set; }

        public string ProdSettingsPath { get; set; }
        
        public string DeploymentArtifactDirectory { get; set; }
    }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WindowsHelper.Services.Notion.BindingModels
{
     public class Parent
        {
            [JsonPropertyName("database_id")]
            public string DatabaseId { get; set; }
        }
    
        public class Url
        {
            [JsonPropertyName("url")]
            public string CourseUri { get; set; }
        }
    
        public class DownloadUrl
        {
            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    
        public class Text
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }
    
        public class Title
        {
            [JsonPropertyName("text")]
            public Text Text { get; set; }
        }
    
        public class Name
        {
            [JsonPropertyName("title")]
            public List<Title> Title { get; set; }
        }
    
        public class Properties
        {
            [JsonPropertyName("Url")]
            public Url Url { get; set; }
    
            [JsonPropertyName("Download Url")]
            public DownloadUrl DownloadUrl { get; set; }
    
            [JsonPropertyName("Name")]
            public Name Name { get; set; }
        }
    
        public class AddNewCourseRequestBindingModel
        {
            [JsonPropertyName("parent")]
            public Parent Parent { get; set; }
    
            [JsonPropertyName("properties")]
            public Properties Properties { get; set; }
        }
}
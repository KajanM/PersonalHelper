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
            public Url(string url)
            {
                CourseUri = url;
            }
            
            [JsonPropertyName("url")]
            public string CourseUri { get; set; }
        }
    
        public class DownloadUrl
        {
            public DownloadUrl(string url)
            {
                Url = url;
            }
            
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
            public Title(string title)
            {
                Text = new Text
                {
                    Content = title
                };
            }
            
            [JsonPropertyName("text")]
            public Text Text { get; set; }
        }
    
        public class Name
        {
            public Name(string name)
            {
               Title.Add(new Title(name)); 
            }

            [JsonPropertyName("title")] public List<Title> Title { get; set; } = new();
        }
    
        public class Properties
        {
            [JsonPropertyName("Url")]
            public Url Url { get; set; }
    
            [JsonPropertyName("Download Url")]
            public DownloadUrl DownloadUrl { get; set; }
    
            [JsonPropertyName("Name")]
            public Name Name { get; set; }

            [JsonPropertyName("Type")]
            public Type Type { get; set; }
        }

        public class Type
        {
            public Type(string name)
            {
                Select = new Select(name);
            }
            
            [JsonPropertyName("select")]
            public Select Select { get; set; } 
        }

        public class Select
        {
            public Select(string name)
            {
                Name = name;
            }
            
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    
        public class AddNewCourseRequestBindingModel
        {
            public AddNewCourseRequestBindingModel(string databaseId, string courseUrl, string downloadUrl, string title)
            {
                Parent = new Parent
                {
                    DatabaseId = databaseId
                };

                Properties = new Properties
                {
                    Url = new Url(courseUrl),
                    DownloadUrl = new DownloadUrl(downloadUrl),
                    Name = new Name(title),
                    Type = new Type("course")
                };
            }
            
            [JsonPropertyName("parent")]
            public Parent Parent { get; set; }
    
            [JsonPropertyName("properties")]
            public Properties Properties { get; set; }
        }
}
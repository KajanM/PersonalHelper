using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WindowsHelper.Shared.Notion.ResponseViewModel
{
    public class Text
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }

    public class Annotations
    {
        [JsonPropertyName("bold")]
        public bool Bold { get; set; }

        [JsonPropertyName("italic")]
        public bool Italic { get; set; }

        [JsonPropertyName("strikethrough")]
        public bool Strikethrough { get; set; }

        [JsonPropertyName("underline")]
        public bool Underline { get; set; }

        [JsonPropertyName("code")]
        public bool Code { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }

    public class Title
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public Text Text { get; set; }

        [JsonPropertyName("annotations")]
        public Annotations Annotations { get; set; }

        [JsonPropertyName("plain_text")]
        public string PlainText { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    public class Url2
    {
    }

    public class Url
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Option
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }

    public class MultiSelect
    {
        [JsonPropertyName("options")]
        public List<Option> Options { get; } = new();
    }

    public class Tags
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("multi_select")]
        public MultiSelect MultiSelect { get; set; }
    }

    public class DownloadUrl
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public Url Url { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string ColumnName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("title")]
        public Title Title { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("Url")]
        public Url Url { get; set; }

        [JsonPropertyName("Tags")]
        public Tags Tags { get; set; }

        [JsonPropertyName("Download Url")]
        public DownloadUrl DownloadUrl { get; set; }

        [JsonPropertyName("Name")]
        public Name Name { get; set; }
    }

    public class Parent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("workspace")]
        public bool Workspace { get; set; }
    }

    public class DescribeDatabaseResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("last_edited_time")]
        public DateTime LastEditedTime { get; set; }

        [JsonPropertyName("title")]
        public List<Title> Title { get; } = new();

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }

        [JsonPropertyName("parent")]
        public Parent Parent { get; set; }
    }
}
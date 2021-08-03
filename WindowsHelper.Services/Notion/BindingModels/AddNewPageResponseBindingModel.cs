using System.Text.Json.Serialization;

namespace WindowsHelper.Services.Notion.BindingModels
{
    public class AddNewPageResponseBindingModel
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
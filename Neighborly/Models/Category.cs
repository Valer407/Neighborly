using System.Text.Json.Serialization;
namespace Neighborly.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        public string IconSvg { get; set; } 
    }
}

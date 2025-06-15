using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Neighborly.Models.DBModels
{
    public class Categories
    {
        [Key]
        [JsonPropertyName("id")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(255)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        public string IconSvg { get; set; }

    }
}
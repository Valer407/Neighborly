using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Neighborly.Models.DBModels
{
    public class Listing_types
    {
        [Key]
        [JsonPropertyName("id")]
        public int ListingTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
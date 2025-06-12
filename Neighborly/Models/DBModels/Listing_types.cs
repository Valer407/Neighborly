using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Listing_types
    {
        [Key]
        public int ListingTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

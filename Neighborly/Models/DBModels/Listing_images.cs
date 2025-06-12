using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Listing_images
    {
            [Key]
            public int ImageId { get; set; }

            [Required]
            public int ListingId { get; set; }

            [Required]
            [MaxLength(512)]
            public string Url { get; set; }

            public int SortOrder { get; set; } = 0;

    }
}

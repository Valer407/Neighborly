using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Neighborly.Models.DBModels
{
    public class ListingPromotion
    {
        [Key]
        public int PromotionId { get; set; }

        [Required]
        public int ListingId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("ListingId")]
        public Listings Listing { get; set; }
    }
}

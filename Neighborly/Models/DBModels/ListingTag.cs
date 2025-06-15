using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Neighborly.Models.DBModels
{
    public class ListingTag
    {
        [Key]
        public int ListingTagId { get; set; }

        [Required]
        public int ListingId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tag { get; set; }

        [ForeignKey("ListingId")]
        public Listings Listing { get; set; }
    }
}

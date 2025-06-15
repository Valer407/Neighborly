using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Neighborly.Models.DBModels
{
    public class ListingView
    {
        [Key]
        public int ListingViewId { get; set; }

        [Required]
        public int ListingId { get; set; }

        public int? ViewerUserId { get; set; }

        [MaxLength(45)] // IPv6
        public string ViewerIp { get; set; }

        [Required]
        public DateTime ViewedAt { get; set; }

        [ForeignKey("ListingId")]
        public Listings Listing { get; set; }

        [ForeignKey("ViewerUserId")]
        public User Viewer { get; set; }
    }
}

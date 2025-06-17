using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models
{
    public class ReportViewModel
    {
        [Required]
        public int ListingId { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; }

        public string? Description { get; set; }
    }
}
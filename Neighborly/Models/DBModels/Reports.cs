using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Reports
    {
            [Key]
            public int ReportId { get; set; }

            [Required]
            public int ReporterId { get; set; }

            [Required]
            public int ListingId { get; set; }

            [Required]
            [MaxLength(255)]
            public string Reason { get; set; }

            public string Description { get; set; }

            [MaxLength(50)]
            public string Status { get; set; } = "open";

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public DateTime CreatedAt { get; set; }

    }
}

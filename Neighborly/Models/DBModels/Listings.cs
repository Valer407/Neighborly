using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Listings
    {
        [Key]
        public int ListingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ListingTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int DistrictId { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "active";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
                [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CategoryId")]
        public Categories Category { get; set; }

        [ForeignKey("ListingTypeId")]
        public Listing_types ListingType { get; set; }

        [ForeignKey("CityId")]
        public Cities City { get; set; }

        [ForeignKey("DistrictId")]
        public Distircts District { get; set; }
    }
}

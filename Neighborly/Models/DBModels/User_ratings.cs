using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class User_ratings
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int RaterId { get; set; }

        [Required]
        public int RateeId { get; set; }

        [Required]
        public int ListingId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public string Comment { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Neighborly.Models.DBModels
{
    public class Favourites
    {
            [Required]
            public int UserId { get; set; }

            [Required]
            public int ListingId { get; set; }

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public DateTime CreatedAt { get; set; }

            public User User { get; set; }
            public Listings Listing { get; set; }

    }
}

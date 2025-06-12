using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Chats
    {
        [Key]
        public int ChatId { get; set; }

        public int? ListingId { get; set; }

        public int? User1Id { get; set; }

        public int? User2Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}

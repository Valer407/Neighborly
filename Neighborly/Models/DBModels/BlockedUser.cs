using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class BlockedUser
    {
        public int UserId { get; set; }
        public int BlockedId { get; set; }

        [Required]
        public DateTime BlockedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("BlockedId")]
        public User BlockedUserRef { get; set; }
    }
}

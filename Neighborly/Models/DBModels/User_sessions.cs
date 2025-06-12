using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class User_sessions
    {
        [Key]
        public Guid SessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(512)]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}

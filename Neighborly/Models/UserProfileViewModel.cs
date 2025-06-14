using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models
{
    public class UserProfileViewModel
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string AvatarUrl { get; set; }

        [MaxLength(255)]
        public string? City { get; set; }

        [MaxLength(255)]
        public string? District { get; set; }

        public string? About { get; set; }
    }
}
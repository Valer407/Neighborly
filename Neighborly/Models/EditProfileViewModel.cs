using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models
{
    public class EditProfileViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? About { get; set; }
    }
}
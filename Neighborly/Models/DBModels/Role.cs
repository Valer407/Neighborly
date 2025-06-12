using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}

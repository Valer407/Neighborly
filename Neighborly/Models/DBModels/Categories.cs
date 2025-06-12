using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

    }
}

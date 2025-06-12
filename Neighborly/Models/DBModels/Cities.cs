using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Cities
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}

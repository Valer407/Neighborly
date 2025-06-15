using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Distircts
    {
        [Key]
        public int DistrictId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [ForeignKey("CityId")]
        public Cities City { get; set; }
    }
}

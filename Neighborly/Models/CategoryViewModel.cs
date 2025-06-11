using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models
{
public class CategoryViewModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Ikona SVG")]
    public string IconSvg { get; set; }
}
}

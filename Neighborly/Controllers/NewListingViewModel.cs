using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models
{
    public class NewListingViewModel
    {
        [Required(ErrorMessage = "Tytuł ogłoszenia jest wymagany.")]
        [StringLength(100, ErrorMessage = "Tytuł może mieć maksymalnie 100 znaków.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        [StringLength(1000, ErrorMessage = "Opis może mieć maksymalnie 1000 znaków.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Wybierz kategorię.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Wybierz rodzaj ogłoszenia.")]
        public string Type { get; set; } // np. "offer" lub "request"

        [Required(ErrorMessage = "Podaj miasto.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Podaj dzielnicę.")]
        public string District { get; set; }
    }
}

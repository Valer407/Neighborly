using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Neighborly.Models.DBModels
{
    public class Messages
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int ChatId { get; set; }

        public int? SenderId { get; set; }

        [Required]
        public string Content { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SentAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}

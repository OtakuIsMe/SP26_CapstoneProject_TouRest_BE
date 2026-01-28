using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("images")]
    public class Image : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        [MaxLength(100)]
        public string? Type { get; set; }

        // Navigation
        public User Users { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("images")]
    public class Image : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = null!;

        [Required]
        public ImageType Type { get; set; }

        // Navigation
        public User Users { get; set; } = null!;
    }
}

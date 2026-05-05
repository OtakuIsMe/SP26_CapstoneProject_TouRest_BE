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

        [Required]
        public Guid TypeId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int PicNumber { get; set; }
        public Guid? PublicByUserId { get; set; }
        public User? User { get; set; }
    }
}

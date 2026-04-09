using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("agencies")]
    public class Agency : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public AgencyStatus Status { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}

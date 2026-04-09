using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("providers")]
    public class Provider : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public ProviderStatus Status { get; set; }

        [Required]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}

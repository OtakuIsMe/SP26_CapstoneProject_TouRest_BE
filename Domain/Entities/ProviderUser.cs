using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("provider_users")]
    public class ProviderUser : BaseEntity
    {
        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public ProviderUserRole Role { get; set; }

        // Navigation properties
        public Provider Provider { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("users")]
    public class User : BaseEntity
    {
        [Required]
        public Guid RoleId { get; set; }

        public Guid? ImageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = null!;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public UserStatus Status { get; set; }

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Image? Image { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("notifications")]
    public class Notification : BaseEntity
    {
        [Required]
        public Guid RecipientUserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = null!;

        [Required]
        public NotificationEntityType EntityType { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        // Navigation properties
        public User RecipientUser { get; set; } = null!;
    }
}

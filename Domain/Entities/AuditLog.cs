using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        [Required]
        public Guid ActorUserId { get; set; }

        [Required]
        public string Action { get; set; } = null!;

        public Guid? TargetUserId { get; set; }

        [MaxLength(100)]
        public string? EntityType { get; set; } 

        public Guid? EntityId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }   

        public string? OldValue { get; set; }    
        public string? NewValue { get; set; }    

        // Navigation
        public User Actor { get; set; } = null!;
        public User? TargetUser { get; set; }
    }
}

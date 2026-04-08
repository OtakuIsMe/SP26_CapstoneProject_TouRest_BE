using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("agency_users")]
    public class AgencyUser : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public bool IsPrimaryContact { get; set; }
        // Navigation properties
        public Agency Agency { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

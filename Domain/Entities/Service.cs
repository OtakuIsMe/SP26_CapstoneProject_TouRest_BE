using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("services")]
    public class Service : BaseEntity
    {
        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DurationMinutes must be greater than 0")]
        public int DurationMinutes { get; set; }

        [Required]
        public ServiceStatus Status { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public int BasePrice { get; set; }

        // Navigation properties
        public Provider Provider { get; set; } = null!;
    }
}

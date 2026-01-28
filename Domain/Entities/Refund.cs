using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("refunds")]
    public class Refund : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public int TotalRefundAmount { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        public RefundStatus Status { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
    }
}

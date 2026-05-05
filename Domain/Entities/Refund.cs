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
        public Guid PaymentId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "TotalRefundAmount must be greater than 0")]
        public long TotalRefundAmount { get; set; }
        public RefundInitinator InitiatedBy { get; set; }
        public string? CustomerBankAccount { get; set; }
        public string? CustomerBankName { get; set; }
        public string? CustomerAccountHolder { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
        public string? AdminNote { get; set; }
        public DateTime? RefundedAt { get; set; }

        [Required]
        public RefundStatus Status { get; set; }

        // Navigation properties
        public Payment Payment { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}

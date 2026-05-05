using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid BookingId { get; set; }
        public long OrderCode { get; set; }
        [Range(1, long.MaxValue)]
        public long Amount { get; set; }
        [Range(1, long.MaxValue)]
        public long DiscountAmount { get; set; }
        [Range(1, long.MaxValue)]
        public long FinalAmount { get; set; }
        public PaymentStatus Status { get; set; }         
        public string? PayOSPaymentLinkId { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? CancelledReason { get; set; }
        public string? TransactionReference { get; set; } 
        public DateTime? PaidAt { get; set; }
        public DateTime ExpiredAt { get; set; }

        // Navigation
        public Booking Booking { get; set; } = null!;
        public Refund? Refund { get; set; }
    }
}

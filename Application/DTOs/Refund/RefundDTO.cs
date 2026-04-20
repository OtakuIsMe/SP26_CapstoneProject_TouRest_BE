using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Refund
{
    public class RefundDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string InitiatedBy { get; set; } = null!;
        public string? Reason { get; set; }
        public string? AdminNote { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? RefundedAt { get; set; }
    }
}

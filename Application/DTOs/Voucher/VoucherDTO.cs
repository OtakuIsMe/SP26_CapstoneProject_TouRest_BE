using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Voucher
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public int DiscountValue { get; set; }
        public int? MaxDiscountAmount { get; set; }
        public int? MinOrderAmount { get; set; }
        public VoucherApplicableType ApplicableType { get; set; }
        public Guid? ApplicableId { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public VoucherStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

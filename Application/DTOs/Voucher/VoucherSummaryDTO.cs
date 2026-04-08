using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Voucher
{
    public class VoucherSummaryDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DiscountType DiscountType { get; set; }
        public int DiscountValue { get; set; }
        public VoucherApplicableType ApplicableType { get; set; }
        public VoucherStatus Status { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}

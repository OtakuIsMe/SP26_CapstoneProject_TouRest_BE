using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Voucher
{
    public class VoucherUpdateRequest
    {
        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "DiscountValue must be between 0 and 100")]
        public int DiscountValue { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxDiscountAmount must be greater than 0")]
        public int? MaxDiscountAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MinOrderAmount must be greater than 0")]
        public int? MinOrderAmount { get; set; }

        [Required]
        public VoucherApplicableType ApplicableType { get; set; }

        public Guid? ApplicableId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsageLimit must be greater than 0")]
        public int? UsageLimit { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public VoucherStatus Status { get; set; }
    }
}

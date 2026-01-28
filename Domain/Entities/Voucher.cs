using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("vouchers")]
    public class Voucher : BaseEntity
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
        public int DiscountValue { get; set; }

        public int? MaxDiscountAmount { get; set; }

        public int? MinOrderAmount { get; set; }

        [Required]
        public VoucherApplicableType ApplicableType { get; set; }

        public Guid? ApplicableId { get; set; }

        public int? UsageLimit { get; set; }

        [Required]
        public int UsedCount { get; set; } = 0;

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public VoucherStatus Status { get; set; }
    }
}

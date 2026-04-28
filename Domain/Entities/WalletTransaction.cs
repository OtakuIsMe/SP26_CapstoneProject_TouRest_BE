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
    [Table("wallet_transactions")]
    public class WalletTransaction : BaseEntity
    {
        public Guid WalletId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public WalletTransactionType Type { get; set; }  // Credit, Debit
        public WalletTransactionReason Reason { get; set; } // BookingEarning, Refund, Payout
        public Guid? ReferenceId { get; set; }  // BookingId, RefundId, PayoutId
        [MaxLength(500)]
        public string? Note { get; set; }

        // Navigation
        public Wallet Wallet { get; set; } = null!;
    }
}

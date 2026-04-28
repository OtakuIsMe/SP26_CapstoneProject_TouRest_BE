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
    [Table("payouts")]
    public class Payout : BaseEntity
    {
        public Guid WalletId { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Amount need to be larger than 0")]
        public long Amount { get; set; }

        public PayoutStatus Status { get; set; } 
        public string? AdminNote { get; set; }
        public string? PayOSTransferId { get; set; }
        public DateTime? PaidAt { get; set; }

        public string BankAccount { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string AccountHolder { get; set; } = null!;

        public Wallet Wallet { get; set; } = null!;
    }
}

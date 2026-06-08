using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Wallet
{
    public class WalletTopUpRequest
    {
        [Required]
        [Range(10_000, 100_000_000, ErrorMessage = "Amount must be between 10,000 and 100,000,000")]
        public long Amount { get; set; }
    }
}

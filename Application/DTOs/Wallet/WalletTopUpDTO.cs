namespace TouRest.Application.DTOs.Wallet
{
    public class WalletTopUpDTO
    {
        public Guid Id { get; set; }
        public long OrderCode { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; } = null!;
        public string? CheckoutUrl { get; set; }
        public string? QrCode { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}

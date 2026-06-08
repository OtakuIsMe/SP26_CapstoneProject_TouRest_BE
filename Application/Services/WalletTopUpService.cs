using PayOS;
using PayOS.Models.V2.PaymentRequests;
using TouRest.Application.DTOs.Wallet;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class WalletTopUpService : IWalletTopUpService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _walletTxRepo;
        private readonly PayOSClient _payOS;

        public WalletTopUpService(
            IWalletRepository walletRepo,
            IWalletTransactionRepository walletTxRepo,
            PayOSClient payOS)
        {
            _walletRepo  = walletRepo;
            _walletTxRepo = walletTxRepo;
            _payOS       = payOS;
        }

        public async Task<WalletTopUpDTO> CreateTopUpAsync(Guid userId, long amount)
        {
            _ = await _walletRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found");

            var orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);
            var appUrl    = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:3000";

            var link = await _payOS.PaymentRequests.CreateAsync(new CreatePaymentLinkRequest
            {
                OrderCode   = orderCode,
                Amount      = (int)amount,
                Description = $"Nap vi {amount / 1000}k",
                CancelUrl   = $"{appUrl}/payment/cancel",
                ReturnUrl   = $"{appUrl}/payment/success",
                ExpiredAt   = (int)DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            });

            return new WalletTopUpDTO
            {
                OrderCode   = orderCode,
                Amount      = amount,
                Status      = "Pending",
                CheckoutUrl = link.CheckoutUrl,
                QrCode      = link.QrCode,
                ExpiredAt   = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public async Task<WalletTopUpDTO> GetStatusAsync(long orderCode)
        {
            // If already credited in our DB, return Paid immediately
            if (await _walletTxRepo.ExistsByTopUpOrderCodeAsync(orderCode))
                return new WalletTopUpDTO { OrderCode = orderCode, Status = "Paid" };

            try
            {
                var info = await _payOS.PaymentRequests.GetAsync(orderCode);
                return new WalletTopUpDTO
                {
                    OrderCode = orderCode,
                    Amount    = info.Amount,
                    Status    = info.Status.ToString(),
                };
            }
            catch
            {
                return new WalletTopUpDTO { OrderCode = orderCode, Status = "Unknown" };
            }
        }

        public async Task FinalizeTopUpByOrderCodeAsync(long orderCode, Guid userId)
        {
            // Double-credit guard
            if (await _walletTxRepo.ExistsByTopUpOrderCodeAsync(orderCode)) return;

            // Verify with PayOS
            PaymentLink info;
            try { info = await _payOS.PaymentRequests.GetAsync(orderCode); }
            catch { return; }

            if (info.Status != PaymentLinkStatus.Paid) return;

            var wallet = await _walletRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found");

            wallet.Balance   += info.Amount;
            wallet.UpdatedAt =  DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet);

            await _walletTxRepo.CreateAsync(new WalletTransaction
            {
                Id        = Guid.NewGuid(),
                WalletId  = wallet.Id,
                Amount    = info.Amount,
                Type      = WalletTransactionType.Credit,
                Reason    = WalletTransactionReason.TopUp,
                Note      = $"TopUp#{orderCode}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
    }
}

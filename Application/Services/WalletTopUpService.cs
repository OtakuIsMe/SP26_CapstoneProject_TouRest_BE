using System.Collections.Concurrent;
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
        // DEV/TEST: PayOS is charged this amount; wallet is credited with the real amount.
        // Set to 0 to disable test mode and charge the real amount.
        private const long TestPaymentAmount = 2_000;

        // Stores orderCode → intended wallet credit amount while payment is pending.
        private static readonly ConcurrentDictionary<long, long> _pendingAmounts = new();

        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _walletTxRepo;
        private readonly PayOSClient _payOS;

        public WalletTopUpService(
            IWalletRepository walletRepo,
            IWalletTransactionRepository walletTxRepo,
            PayOSClient payOS)
        {
            _walletRepo   = walletRepo;
            _walletTxRepo = walletTxRepo;
            _payOS        = payOS;
        }

        public async Task<WalletTopUpDTO> CreateTopUpAsync(Guid userId, long amount)
        {
            _ = await _walletRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found");

            var orderCode   = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);
            var appUrl      = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:3000";
            var chargeAmount = TestPaymentAmount > 0 ? TestPaymentAmount : amount;

            _pendingAmounts[orderCode] = amount;

            var link = await _payOS.PaymentRequests.CreateAsync(new CreatePaymentLinkRequest
            {
                OrderCode   = orderCode,
                Amount      = (int)chargeAmount,
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
            if (await _walletTxRepo.ExistsByTopUpOrderCodeAsync(orderCode))
                return new WalletTopUpDTO { OrderCode = orderCode, Status = "Paid" };

            try
            {
                var info = await _payOS.PaymentRequests.GetAsync(orderCode);
                var realAmount = _pendingAmounts.TryGetValue(orderCode, out var stored) ? stored : info.Amount;
                return new WalletTopUpDTO
                {
                    OrderCode = orderCode,
                    Amount    = realAmount,
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
            if (await _walletTxRepo.ExistsByTopUpOrderCodeAsync(orderCode)) return;

            PaymentLink info;
            try { info = await _payOS.PaymentRequests.GetAsync(orderCode); }
            catch { return; }

            if (info.Status != PaymentLinkStatus.Paid) return;

            // Use stored real amount; fall back to what PayOS reported if the server restarted.
            var creditAmount = _pendingAmounts.TryGetValue(orderCode, out var stored) ? stored : info.Amount;

            var wallet = await _walletRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Wallet not found");

            wallet.Balance   += creditAmount;
            wallet.UpdatedAt  = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet);

            await _walletTxRepo.CreateAsync(new WalletTransaction
            {
                Id        = Guid.NewGuid(),
                WalletId  = wallet.Id,
                Amount    = creditAmount,
                Type      = WalletTransactionType.Credit,
                Reason    = WalletTransactionReason.TopUp,
                Note      = $"TopUp#{orderCode}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            _pendingAmounts.TryRemove(orderCode, out _);
        }
    }
}

using AutoMapper;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Payment;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly PayOSClient _payOS;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IBookingItineraryRepository bookingItineraryRepository,
            INotificationRepository notificationRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            PayOSClient payOS,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _bookingItineraryRepository = bookingItineraryRepository;
            _notificationRepository = notificationRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _payOS = payOS;
            _mapper = mapper;
        }

        public async Task<PaymentDTO> CreatePaymentAsync(Guid bookingId, Guid userId)
        {
            var booking = await ValidateBookingOwnership(bookingId, userId);
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in a payable state");


            var existingPayment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (existingPayment != null)
            {
                await CancelPayOSLinkAsync(existingPayment);
                existingPayment.Status = PaymentStatus.Cancelled;
                await _paymentRepository.UpdateAsync(existingPayment);
            }


            var grossAmount    = booking.BookingItineraries.Sum(bi => bi.Price);
            var discountAmount = booking.BookingItineraries.Sum(bi => bi.Price - bi.FinalPrice);
            var finalAmount    = booking.TotalAmount;


            var orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);


            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = 2000, // test-only: fixed 2,000₫ sent to PayOS; real amount stored in DB
                Description = $"Booking {booking.Code}",
                CancelUrl = $"{Environment.GetEnvironmentVariable("APP_URL")}/payment/cancel",
                ReturnUrl = $"{Environment.GetEnvironmentVariable("APP_URL")}/payment/success",
                ExpiredAt = (int)DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds()
            };

            var paymentLink = await _payOS.PaymentRequests.CreateAsync(paymentRequest);


            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                OrderCode = orderCode,
                Amount = grossAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                Status = PaymentStatus.Pending,
                PayOSPaymentLinkId = paymentLink.PaymentLinkId,
                CheckoutUrl = paymentLink.CheckoutUrl,
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _paymentRepository.CreateAsync(payment);
            var dto = _mapper.Map<PaymentDTO>(created);
            dto.QrCode = paymentLink.QrCode;  // ← gán trực tiếp
            return dto;
        }

        public async Task<PaymentDTO> PayWithWalletAsync(Guid bookingId, Guid userId)
        {
            var booking = await ValidateBookingOwnership(bookingId, userId);
            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException("Booking is not in a payable state");

            var existingPayment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (existingPayment != null)
            {
                await CancelPayOSLinkAsync(existingPayment);
                existingPayment.Status = PaymentStatus.Cancelled;
                await _paymentRepository.UpdateAsync(existingPayment);
            }

            // Booking payment uses customer's personal wallet, not agency/provider wallet
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
                throw new KeyNotFoundException("Wallet not found");

            var finalAmount = booking.TotalAmount;
            if (wallet.Balance < finalAmount)
                throw new InvalidOperationException("Insufficient wallet balance");

            wallet.Balance -= finalAmount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);

            var walletTransaction = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                Amount = finalAmount,
                Type = WalletTransactionType.Debit,
                Reason = WalletTransactionReason.BookingPayment,
                ReferenceId = bookingId,
                Note = $"Payment for booking {booking.Code} using wallet",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _walletTransactionRepository.CreateAsync(walletTransaction);

            var orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                OrderCode = orderCode,
                Amount = booking.BookingItineraries.Sum(bi => bi.Price),
                DiscountAmount = booking.BookingItineraries.Sum(bi => bi.Price - bi.FinalPrice),
                FinalAmount = finalAmount,
                Status = PaymentStatus.Paid,
                TransactionReference = $"WALLET-{walletTransaction.Id}",
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CheckoutUrl = null,
                PayOSPaymentLinkId = null,
                PaidAt = DateTime.UtcNow
            };

            var created = await _paymentRepository.CreateAsync(payment);
            await ConfirmBookingAsync(payment, booking);

            return _mapper.Map<PaymentDTO>(created);
        }

        public async Task HandleWebhookAsync(Webhook webhookData)
        {
            // Verify signature — throws if invalid
            var verifiedData = await _payOS.Webhooks.VerifyAsync(webhookData);

            var payment = await _paymentRepository.GetByOrderCodeAsync(verifiedData.OrderCode);
            if (payment == null) return;

            if (verifiedData.Code == "00") // success
            {
                payment.Status = PaymentStatus.Paid;
                payment.TransactionReference = verifiedData.Reference;
                payment.PaidAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);

                // Confirm booking + update payment status
                var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
                if (booking != null)
                {
                    await ConfirmBookingAsync(payment, booking);
                }
            }
            else // failed or cancelled
            {
                payment.Status    = PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
            }
        }

        public async Task<PaymentDTO> CancelPaymentAsync(Guid bookingId, Guid userId)
        {
            await ValidateBookingOwnership(bookingId, userId);
            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (payment == null)
                throw new KeyNotFoundException("No active payment found");

            await CancelPayOSLinkAsync(payment);
            payment.Status = PaymentStatus.Cancelled;
            payment.CancelledReason = "Cancelled by user";
            payment.UpdatedAt = DateTime.UtcNow;

            var updated = await _paymentRepository.UpdateAsync(payment);
            return _mapper.Map<PaymentDTO>(updated);
        }

        public async Task<PaymentDTO> GetActivePaymentAsync(Guid bookingId)
        {
            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(bookingId);
            if (payment == null)
                throw new KeyNotFoundException("No active payment found");
            return _mapper.Map<PaymentDTO>(payment);
        }

        public async Task<PaymentDTO?> GetLatestPaymentAsync(Guid bookingId)
        {
            var payment = await _paymentRepository.GetLatestPaymentByBookingIdAsync(bookingId);
            return payment == null ? null : _mapper.Map<PaymentDTO>(payment);
        }

        public async Task FinalizePaymentByOrderCodeAsync(long orderCode)
        {
            var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode);
            if (payment == null || payment.Status == PaymentStatus.Paid) return;

            // Verify with PayOS API
            PayOS.Models.V2.PaymentRequests.PaymentLink paymentInfo;
            try { paymentInfo = await _payOS.PaymentRequests.GetAsync(orderCode); }
            catch { return; }

            if (paymentInfo.Status != PayOS.Models.V2.PaymentRequests.PaymentLinkStatus.Paid) return;

            payment.Status    = PaymentStatus.Paid;
            payment.PaidAt    = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment);

            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            if (booking == null) return;

            await ConfirmBookingAsync(payment, booking);
        }

        private async Task CancelPayOSLinkAsync(Payment payment)
        {
            try
            {
                if (!string.IsNullOrEmpty(payment.PayOSPaymentLinkId))
                    await _payOS.PaymentRequests.CancelAsync(payment.PayOSPaymentLinkId);
            }
            catch (Exception)
            {

            }
        }

        private async Task ConfirmBookingAsync(Payment payment, Booking booking)
        {
            booking.Status = BookingStatus.Confirmed;
            booking.PaymentStatus = PaymentStatus.Paid;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            var lines = await _bookingItineraryRepository.GetBookingItinerariesByBookingId(payment.BookingId);
            foreach (var line in lines)
            {
                line.Status = BookingItineraryStatus.Confirmed;
                line.UpdatedAt = DateTime.UtcNow;
                await _bookingItineraryRepository.UpdateAsync(line);
            }

            await _notificationRepository.CreateAsync(new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = booking.UserId,
                Title = "Payment Confirmed",
                Message = $"Your payment of {payment.FinalAmount:N0}đ for booking #{booking.Code} has been confirmed. Funds are held until the tour is completed.",
                EntityType = NotificationEntityType.Booking,
                EntityId = payment.BookingId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
            });
        }

        private async Task<Booking> ValidateBookingOwnership(Guid bookingId, Guid userId)
        {
            var booking = await _bookingRepository.GetBookingWithItineraries(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            return booking;
        }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Refund;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public RefundService(
            IRefundRepository refundRepository,
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IMapper mapper)
        {
            _refundRepository = refundRepository;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<RefundDTO> RequestRefundAsync(RefundRequestDTO request, Guid userId)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found");
            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this booking");
            if (booking.Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be refunded");

            var payment = await _paymentRepository.GetActivePaymentByBookingIdAsync(request.BookingId);
            if (payment == null || payment.Status != PaymentStatus.Paid)
                throw new InvalidOperationException("No completed payment found for this booking");

            var existing = await _refundRepository.GetByPaymentIdAsync(payment.Id);
            if (existing != null)
                throw new InvalidOperationException("A refund already exists for this payment");

            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                PaymentId = payment.Id,
                TotalRefundAmount = payment.FinalAmount,
                InitiatedBy = RefundInitinator.Customer,
                Reason = request.Reason,
                CustomerBankAccount = request.CustomerBankAccount,
                CustomerBankName = request.CustomerBankName,
                CustomerAccountHolder = request.CustomerAccountHolder,
                Status = RefundStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _refundRepository.CreateAsync(refund);
            return _mapper.Map<RefundDTO>(created);
        }

        public async Task<RefundDTO> ReviewRefundAsync(Guid refundId, RefundReviewDTO review)
        {
            var refund = await CheckRefund(refundId);
            if (refund.Status != RefundStatus.Pending)
                throw new InvalidOperationException("Refund is not pending");

            refund.Status = review.Approved ? RefundStatus.Approved : RefundStatus.Rejected;
            refund.AdminNote = review.AdminNote;
            refund.UpdatedAt = DateTime.UtcNow;

            var updated = await _refundRepository.UpdateAsync(refund);
            return _mapper.Map<RefundDTO>(updated);
        }

        public async Task<RefundDTO> CompleteRefundAsync(Guid refundId)
        {
            var refund = await CheckRefund(refundId);
            if (refund.Status != RefundStatus.Approved)
                throw new InvalidOperationException("Refund must be approved before completing");

            refund.Status = RefundStatus.Completed;
            refund.RefundedAt = DateTime.UtcNow;
            refund.UpdatedAt = DateTime.UtcNow;

            // Cancel the booking
            var booking = await _bookingRepository.GetByIdAsync(refund.BookingId);
            if (booking != null)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;
                await _bookingRepository.UpdateAsync(booking);
            }

            var updated = await _refundRepository.UpdateAsync(refund);
            return _mapper.Map<RefundDTO>(updated);
        }

        public async Task<RefundDTO> GetRefundByBookingAsync(Guid bookingId)
        {
            var refund = await _refundRepository.GetByBookingIdAsync(bookingId);
            if (refund == null)
                throw new KeyNotFoundException("Refund not found");
            return _mapper.Map<RefundDTO>(refund);
        }

        private async Task<Refund> CheckRefund(Guid refundId)
        {
            var refund = await _refundRepository.GetByIdAsync(refundId);
            if (refund == null)
                throw new KeyNotFoundException("Refund not found");
            return refund;
        }
    }
}

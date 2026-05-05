using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IBookingItineraryRepository _bookingItineraryRepo;
        private readonly IItineraryScheduleRepository _scheduleRepo;
        private readonly IItineraryRepository _itineraryRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepo,
            IBookingItineraryRepository bookingItineraryRepo,
            IItineraryScheduleRepository scheduleRepo,
            IItineraryRepository itineraryRepo,
            IVoucherRepository voucherRepo,
            INotificationRepository notificationRepo,
            IMapper mapper)
        {
            _bookingRepo          = bookingRepo;
            _bookingItineraryRepo = bookingItineraryRepo;
            _scheduleRepo         = scheduleRepo;
            _itineraryRepo        = itineraryRepo;
            _voucherRepo          = voucherRepo;
            _notificationRepo     = notificationRepo;
            _mapper               = mapper;
        }

        public async Task<BookingDTO> GetBookingAsync(Guid id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found");
            CheckUserOwnTheBooking(booking, userId, isAdmin);
            return _mapper.Map<BookingDTO>(booking);
        }
        public async Task<BookingDTO> CreateBookingAsync(BookingCreateRequest request, Guid userId)
        {
            var booking = _mapper.Map<Booking>(request);
            booking.Id = Guid.NewGuid();
            booking.UserId = userId;
            booking.Code = $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            booking.TotalAmount = 0;
            booking.Status = BookingStatus.Pending;
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            var createdBooking = await _bookingRepo.CreateAsync(booking);
            return _mapper.Map<BookingDTO>(createdBooking);
        }

        public async Task<BookingCreateResponse> CreateBookingAsync(BookingCreateRequest request, Guid userId)
        {
            // 1. Load schedule
            var schedule = await _scheduleRepo.GetByIdAsync(request.ScheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.SpotLeft < request.NumberOfPeople)
                throw new InvalidOperationException(
                    $"Not enough spots. Available: {schedule.SpotLeft}, requested: {request.NumberOfPeople}");

            // 2. Load itinerary for base price
            var itinerary = await _itineraryRepo.GetByIdAsync(schedule.ItineraryId)
                ?? throw new KeyNotFoundException("Itinerary not found");

            int baseAmount = itinerary.Price * request.NumberOfPeople;

            // 3. Validate voucher
            var (voucher, voucherError) = await ValidateVoucherAsync(request.VoucherCode, baseAmount);
            if (voucherError != null)
                throw new ArgumentException(voucherError);

            // 4. Apply discount
            int discountAmount = CalculateDiscount(baseAmount, voucher);
            int totalAmount    = baseAmount - discountAmount;

            // 5. Create booking
            var bookingCode = GenerateCode();
            var booking = new Booking
            {
                Id            = Guid.NewGuid(),
                UserId        = userId,
                Code          = bookingCode,
                TotalAmount   = totalAmount,
                Status        = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                CustomerNote  = request.CustomerNote,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow,
            };
            await _bookingRepo.CreateAsync(booking);

            // 6. Create booking itinerary line
            var line = new BookingItinerary
            {
                Id                  = Guid.NewGuid(),
                BookingId           = booking.Id,
                ItineraryScheduleId = schedule.Id,
                VoucherId           = voucher?.Id,
                Price               = itinerary.Price,
                NumberOfPeople      = request.NumberOfPeople,
                Status              = BookingItineraryStatus.Pending,
                CreatedAt           = DateTime.UtcNow,
                UpdatedAt           = DateTime.UtcNow,
            };
            await _bookingItineraryRepo.CreateAsync(line);

            // 7. Decrease schedule spots
            schedule.SpotLeft -= request.NumberOfPeople;
            await _scheduleRepo.UpdateAsync(schedule);

            // 8. Increment voucher usage
            if (voucher != null)
            {
                voucher.UsedCount++;
                await _voucherRepo.UpdateAsync(voucher);
            }

            return new BookingCreateResponse
            {
                BookingId      = booking.Id,
                Code           = bookingCode,
                BaseAmount     = baseAmount,
                DiscountAmount = discountAmount,
                TotalAmount    = totalAmount,
                VoucherApplied = voucher?.Code,
            };
        }

        public async Task<BookingDTO> UpdateBookingAsync(Guid id, Guid userId, bool isAdmin, BookingUpdateRequest request)
        {
            var existingBooking = await CheckBooking(id);
            CheckUserOwnTheBooking(existingBooking, userId, isAdmin);
            _mapper.Map(request, existingBooking);
            existingBooking.UpdatedAt = DateTime.UtcNow;
            var updatedBooking = await _bookingRepository.UpdateAsync(existingBooking);
            return _mapper.Map<BookingDTO>(updatedBooking);
        }

        public async Task DeleteBookingAsync(Guid id, Guid userId, bool isAdmin)
        {
            var existingBooking = await _bookingRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Booking not found");
            CheckUserOwnTheBooking(existingBooking, userId, isAdmin);
            existingBooking.Status = BookingStatus.Cancelled;
            existingBooking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepo.UpdateAsync(existingBooking);
        }
        // ── Helpers ──────────────────────────────────────────────────────────

        private async Task<(Voucher? voucher, string? error)> ValidateVoucherAsync(string? code, int baseAmount)
        {
            if (string.IsNullOrWhiteSpace(code)) return (null, null);

            var voucher = await _voucherRepo.GetByCodeAsync(code);
            if (voucher == null)
                return (null, "Voucher không tồn tại");

            if (voucher.Status != VoucherStatus.Active)
                return (null, "Voucher không còn hiệu lực");

            var now = DateTime.UtcNow;
            if (now < voucher.ValidFrom || now > voucher.ValidTo)
                return (null, "Voucher đã hết hạn");

            if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit.Value)
                return (null, "Voucher đã hết lượt sử dụng");

            if (voucher.MinOrderAmount.HasValue && baseAmount < voucher.MinOrderAmount.Value)
                return (null, $"Đơn hàng tối thiểu {voucher.MinOrderAmount.Value:N0}đ để dùng voucher này");

            return (voucher, null);
>>>>>>> main
        }

        private static int CalculateDiscount(int baseAmount, Voucher? voucher)
        {
            if (voucher == null) return 0;

            int discount = voucher.DiscountType == DiscountType.Percent
                ? (int)(baseAmount * voucher.DiscountValue / 100.0)
                : voucher.DiscountValue;

            if (voucher.MaxDiscountAmount.HasValue)
                discount = Math.Min(discount, voucher.MaxDiscountAmount.Value);

            return Math.Min(discount, baseAmount);
        }

        private static string GenerateCode() =>
            $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}

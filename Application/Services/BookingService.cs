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
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public BookingService(IBookingRepository bookingRepository, INotificationRepository notificationRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }
        public async Task<BookingDTO> GetBookingAsync(Guid id, Guid userId, bool isAdmin)
        {
            var booking = await CheckBooking(id);
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
            var createdBooking = await _bookingRepository.CreateAsync(booking);
            return _mapper.Map<BookingDTO>(createdBooking);
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
            var existingBooking = await CheckBooking(id);
            CheckUserOwnTheBooking(existingBooking, userId, isAdmin);
            existingBooking.Status = BookingStatus.Cancelled;
            existingBooking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(existingBooking);
        }
        public async Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId)
        {

            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);

            return _mapper.Map<List<BookingDTO>>(bookings);
        }
        private async Task<Booking> CheckBooking(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }
            return booking;
        }
        private void CheckUserOwnTheBooking(Booking booking, Guid userId, bool isAdmin)
        {
            if (!isAdmin)
                if (booking.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You do not own this booking");
                }
        }
    }
}

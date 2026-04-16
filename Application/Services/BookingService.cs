using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;
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
        public async Task<BookingDTO> GetBookingAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
               throw new Exception("Booking not found");
            }
            return _mapper.Map<BookingDTO>(booking);
        }
        public async Task<BookingDTO> CreateBookingAsync(BookingCreateRequest request)
        {
            var booking = _mapper.Map<Domain.Entities.Booking>(request);
            var createdBooking = await _bookingRepository.CreateAsync(booking);
            return _mapper.Map<BookingDTO>(createdBooking);
        }
        public async Task<BookingDTO> UpdateBookingAsync(Guid id, BookingUpdateRequest request)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                throw new Exception("Booking not found");
            }
            _mapper.Map(request, existingBooking);
            var updatedBooking = await _bookingRepository.UpdateAsync(existingBooking);
            return _mapper.Map<BookingDTO>(updatedBooking);
        }
        public async Task<bool> DeleteBookingAsync(Guid id)
        {
            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                throw new Exception("Booking not found");
            }
            return await _bookingRepository.DeleteAsync(id);
        }
        public async Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return _mapper.Map<List<BookingDTO>>(bookings);
        }
        }
    }

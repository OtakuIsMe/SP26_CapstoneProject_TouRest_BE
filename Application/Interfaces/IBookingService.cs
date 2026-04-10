using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Booking;

namespace TouRest.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDTO> GetBookingAsync(Guid id);
        Task<BookingDTO> CreateBookingAsync(BookingCreateRequest request);
        Task<BookingDTO> UpdateBookingAsync(Guid id, BookingUpdateRequest request);
        Task<bool> DeleteBookingAsync(Guid id);
        Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId);
    }
}

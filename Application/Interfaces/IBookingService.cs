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
<<<<<<< HEAD
        Task<BookingDTO> CreateBookingAsync(BookingCreateRequest request, Guid userId);
=======
        Task<BookingCreateResponse> CreateBookingAsync(BookingCreateRequest request, Guid userId);
>>>>>>> main
        Task<BookingDTO> UpdateBookingAsync(Guid id, BookingUpdateRequest request);
        Task DeleteBookingAsync(Guid id);
        Task<List<BookingDTO>> GetBookingsByUserIdAsync(Guid userId);
    }
}

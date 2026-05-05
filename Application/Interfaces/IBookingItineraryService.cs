using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.BookingItinerary;

namespace TouRest.Application.Interfaces
{
    public interface IBookingItineraryService
    {
        Task<List<BookingItineraryDTO>> GetBookingItinerariesByBookingId(Guid bookingId);
        Task<BookingItineraryDTO?> GetBookingItinerary(Guid id);
        Task<BookingItineraryDTO> CreateBookingItinerary(Guid userId, BookingItineraryCreateRequest create);
        Task<BookingItineraryDTO> UpdateBookingItinerary(Guid id, Guid userId, BookingItineraryUpdateRequest update);
        Task DeleteBookingItinerary(Guid id, Guid userId);
    }
}

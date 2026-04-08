using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class BookingItineraryService : IBookingItineraryService
    {
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        public BookingItineraryService(IBookingItineraryRepository bookingItineraryRepository)
        {
            _bookingItineraryRepository = bookingItineraryRepository;
        }
        public async Task<List<BookingItinerary>> GetBookingItinerariesByBookingId(Guid bookingId)
        {
            return await _bookingItineraryRepository.GetBookingItinerariesByBookingId(bookingId);
        }
         public async Task<BookingItinerary?> GetBookingItinerary(Guid id)
        {
            return await _bookingItineraryRepository.GetByIdAsync(id);
        }
         public async Task<BookingItinerary> CreateBookingItinerary(BookingItinerary bookingItinerary)
        {
            return await _bookingItineraryRepository.CreateAsync(bookingItinerary);
        }
         public async Task<BookingItinerary> UpdateBookingItinerary(BookingItinerary bookingItinerary)
        {
            return await _bookingItineraryRepository.UpdateAsync(bookingItinerary);
        }
         public async Task<bool> DeleteBookingItinerary(Guid id)
        {
            return await _bookingItineraryRepository.DeleteAsync(id);
        }
    }
}

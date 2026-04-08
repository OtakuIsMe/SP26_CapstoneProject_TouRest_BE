using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class BookingItineraryRepository : BaseRepository<BookingItinerary>, IBookingItineraryRepository
    {
        public BookingItineraryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<BookingItinerary>> GetBookingItinerariesByBookingId(Guid bookingId)
        {
            return await _context.BookingItineraries.Where(x => x.BookingId == bookingId).ToListAsync();
        }
    }

}

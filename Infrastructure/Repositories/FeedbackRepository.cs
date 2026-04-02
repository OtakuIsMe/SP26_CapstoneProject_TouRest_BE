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
    public class FeedbackRepository : BaseRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Feedback>> GetFeedbacksByBookingItineraryIdAsync(Guid bookingItineraryId)
        {
            return await _context.Feedbacks
                .Where(f => f.BookingItineraryId == bookingItineraryId)
                .Include(x => x.BookingItinerary)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Feedback?> GetFeedback(Guid id)
        {
            return await _context.Feedbacks
                .Include(x => x.BookingItinerary)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
        public async Task<List<Feedback>> GetFeedbacks()
        {
            return await _context.Feedbacks
                .Include(x => x.BookingItinerary)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Feedback>> GetFeedbacks(FeedbackSearch search)
        {
            var query = _context.Feedbacks
                .Include(x => x.BookingItinerary).ThenInclude(bi => bi.Booking)
                .AsNoTracking()
                .AsQueryable();
            if(!string.IsNullOrEmpty(search.BookingCode))
            {
                query = query.Where(f => f.BookingItinerary.Booking.Code.Contains(search.BookingCode));
            }
            if(search.ItemType.HasValue)
            {
                query = query.Where(f => f.ItemType == search.ItemType.Value);
            }
            if(search.Rating.HasValue)
            {
                query = query.Where(f => f.Rating == search.Rating.Value);
            }
            if(!string.IsNullOrEmpty(search.Title))
            {
                query = query.Where(f => f.Title.Contains(search.Title));
            }
            if(search.IsAnonymous.HasValue)
            {
                query = query.Where(f => f.IsAnonymous == search.IsAnonymous.Value);
            }
            if(search.Status.HasValue)
            {
                query = query.Where(f => f.Status == search.Status.Value);
            }
            return await query.ToListAsync();
        }
    }
}

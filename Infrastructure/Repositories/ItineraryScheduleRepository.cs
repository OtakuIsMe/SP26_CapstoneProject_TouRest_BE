using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryScheduleRepository : BaseRepository<ItinerarySchedule>, IItineraryScheduleRepository
    {
        public ItineraryScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<List<ItinerarySchedule>> GetByItineraryIdAsync(Guid itineraryId)
        {
            return await _context.ItinerarySchedules
                .Where(s => s.ItineraryId == itineraryId)
                .OrderBy(s => s.StartTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

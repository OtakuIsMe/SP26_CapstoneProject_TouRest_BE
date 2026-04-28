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
    public class ItineraryScheduleRepository : BaseRepository<ItinerarySchedule>, IItineraryScheduleRepository
    {
        public ItineraryScheduleRepository(AppDbContext appDbContext) : base(appDbContext) { }
        public async Task<ItinerarySchedule?> GetScheduleWithDetails(Guid scheduleId)
        {
            return await _context.ItinerarySchedules
                .Include(s => s.Itinerary)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryActivityRepository : BaseRepository<ItineraryActivity>, IItineraryActivityRepository
    {
        public ItineraryActivityRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<ItineraryActivity>> GetActivitiesByItineraryStopId(Guid itineraryStopId)
        {
            return await _context.ItineraryActivities
                .Where(x => x.ItineraryStopId == itineraryStopId).Include(x=>x.Service).AsNoTracking().ToListAsync();
        }
        public async Task<ItineraryActivity?> GetItineraryActivity(Guid id)
        {
            return await _context.ItineraryActivities
                .Include(x => x.Service)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<ItineraryActivity>> GetActivities()
        {
            return await _context.ItineraryActivities
                .Include(x => x.Service)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ItineraryActivity>> GetActivities(ItineraryActivitySearch search)
        {
            var query = _context.ItineraryActivities
                .Include(x => x.Service)
                .Include(x=>x.ItineraryStop)
                .AsNoTracking()
                .AsQueryable();
            if (!string.IsNullOrEmpty(search.ItineraryStopName))
            {
                query = query.Where(x => x.ItineraryStop.Name == search.ItineraryStopName);
            }
            if (!string.IsNullOrEmpty(search.ServiceName))
            {
                query = query.Where(x => x.Service.Name == search.ServiceName);
            }
/*            if (search.ActivityOrder.HasValue)
            {
                query = query.Where(x => x.ActivityOrder == search.ActivityOrder.Value);
            }*/
            if(search.StartTime.HasValue)
            {
                query = query.Where(x => x.StartTime >= search.StartTime.Value);
            }
            if(search.EndTime.HasValue)
            {
                query = query.Where(x => x.EndTime <= search.EndTime.Value);
            }
            if(search.LowPrice.HasValue)
            {
                query = query.Where(x => x.Price >= search.LowPrice.Value);
            }
            if(search.HighPrice.HasValue)
            {
                query = query.Where(x => x.Price <= search.HighPrice.Value);
            }
            return await query.ToListAsync();
        }
    }
}

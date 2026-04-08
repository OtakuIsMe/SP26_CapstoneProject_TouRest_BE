using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TouRest.Infrastructure.Repositories
{
    public class ItineraryRepository : BaseRepository<Itinerary>, IItineraryRepository
    {
        public ItineraryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Itinerary>> GetItineraries(ItinerarySearch search)
        {
            var query = _context.Itineraries.Include(x => x.Agency).AsNoTracking().AsQueryable();

            if (search.AgencyName != null)
                query = query.Where(x => x.Agency.Name == search.AgencyName);

            if (!string.IsNullOrWhiteSpace(search.Name))
                query = query.Where(x => EF.Functions.Like(x.Name, $"%{search.Name}%"));

            if (search.LowPrice != null)
            {
                query = query.Where(x => x.Price >= search.LowPrice);
            }
            if (search.HighPrice != null)
            {
                query = query.Where(x => x.Price <= search.HighPrice);
            }
            if (search.LowDurationDay != null)
                query = query.Where(x => x.DurationDays >= search.LowDurationDay);
            if (search.HighDurationDay != null)
                query = query.Where(x => x.DurationDays <= search.HighDurationDay);

            return await query.ToListAsync();
        }
        public override async Task<Itinerary?> GetByIdAsync(Guid id)
        {
            return await _context.Itineraries
                .Include(x => x.Agency)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        
    }
}

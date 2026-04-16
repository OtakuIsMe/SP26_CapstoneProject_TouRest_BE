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
    public class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(AppDbContext context) : base(context) { }


        public async Task<Agency?> GetByContactEmailAsync(string contactEmail)
        {
            return await _context.Agencies
                .FirstOrDefaultAsync(a => a.ContactEmail == contactEmail);
        }
        public async Task<Agency?> GetMyAgency(Guid userId)
        {
            return await _context.Agencies.FirstOrDefaultAsync(a => a.CreateByUserId == userId);
        }
    }
}

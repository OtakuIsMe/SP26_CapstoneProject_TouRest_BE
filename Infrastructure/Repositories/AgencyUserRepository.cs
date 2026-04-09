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
    public class AgencyUserRepository : BaseRepository<AgencyUser>, IAgencyUserRepository
    {
        public AgencyUserRepository(AppDbContext context) : base(context) { }
        public async Task<bool> IsUserInAgencyAsync(Guid userId, Guid agencyId)
        {
            return await _context.AgencyUsers.AnyAsync(au => au.UserId == userId && au.AgencyId == agencyId);
        }
        public async Task AddUserToAgencyAsync(Guid agencyId, Guid userId)
        {
            if (!await IsUserInAgencyAsync(userId, agencyId))
            {
                var agencyUser = new AgencyUser { AgencyId = agencyId, UserId = userId };
                await _context.AgencyUsers.AddAsync(agencyUser);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId)
        {
            var agencyUser = await _context.AgencyUsers
                .FirstOrDefaultAsync(au => au.UserId == userId && au.AgencyId == agencyId);
            if (agencyUser != null)
            {
                _context.AgencyUsers.Remove(agencyUser);
                await _context.SaveChangesAsync();
            }
        }
    }
}

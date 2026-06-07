using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderUserRepository : BaseRepository<ProviderUser>, IProviderUserRepository
    {
        public ProviderUserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AddUserIntoProvider(Guid providerId, Guid userId, ProviderUserRole role)
        {
            if (!await IsUserInProviderAsync(providerId, userId))
            {
                var providerUser = new ProviderUser { ProviderId = providerId, UserId = userId, Role = role };
                await _context.ProviderUsers.AddAsync(providerUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ProviderUser?> GetByUserIdAsync(Guid userId)
        {
            return await _context.ProviderUsers.Include(x => x.Provider).Include(x => x.User)
                .FirstOrDefaultAsync(pu => pu.UserId == userId);
        }

        public async Task<bool> IsUserInProviderAsync(Guid providerId, Guid userId)
        {
            return await _context.ProviderUsers.AnyAsync(pu => pu.ProviderId == providerId && pu.UserId == userId);
        }

        public async Task RemoveUserFromProviderAsync(Guid providerId, Guid userId)
        {
            var providerUser = await _context.ProviderUsers
                .FirstOrDefaultAsync(pu => pu.ProviderId == providerId && pu.UserId == userId);
            if (providerUser != null)
            {
                _context.ProviderUsers.Remove(providerUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProviderUser>> GetStaffByProviderIdAsync(Guid providerId)
        {
            return await _context.ProviderUsers
                .Include(pu => pu.User)
                .Where(pu => pu.ProviderId == providerId && pu.Role == ProviderUserRole.Staff)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ProviderUser>> GetManagersByProviderIdAsync(Guid providerId)
        {
            return await _context.ProviderUsers
                .Include(pu => pu.User)
                .Where(pu => pu.ProviderId == providerId && pu.Role == ProviderUserRole.Manager)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

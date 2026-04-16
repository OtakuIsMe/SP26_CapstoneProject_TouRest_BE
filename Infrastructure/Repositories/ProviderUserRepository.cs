using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderUserRepository : IProviderUserRepository
    {
        private readonly AppDbContext _context;

        public ProviderUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserIntoProvider(Guid providerId, Guid userId, ProviderUserRole role)
        {
            var providerUser = new ProviderUser { ProviderId = providerId, UserId = userId, Role = role };
            await _context.ProviderUsers.AddAsync(providerUser);
            await _context.SaveChangesAsync();
        }
    }
}

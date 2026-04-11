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
    public class ProviderUserRepository : IProviderUserRepository
    {
        private readonly AppDbContext _context;

        public ProviderUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProviderUser providerUser)
        {
            await _context.ProviderUsers.AddAsync(providerUser);
        }
    }
}

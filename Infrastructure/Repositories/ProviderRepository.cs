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
    public class ProviderRepository : IProviderRepository
    {
        private readonly AppDbContext _context;

        public ProviderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Provider>> GetAllAsync()
        {
            return await _context.Providers
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<Provider?> GetByIdAsync(Guid id)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Provider?> GetByContactEmailAsync(string contactEmail)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(x => x.ContactEmail == contactEmail);
        }

        public async Task<Provider?> GetByCreateByUserIdAsync(Guid userId)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(x => x.CreateByUserId == userId);
        }

        public async Task<bool> ExistsByContactEmailAsync(string contactEmail)
        {
            return await _context.Providers
                .AnyAsync(x => x.ContactEmail == contactEmail);
        }

        public async Task AddAsync(Provider provider)
        {
            await _context.Providers.AddAsync(provider);
        }

        public void Update(Provider provider)
        {
            _context.Providers.Update(provider);
        }

        public void Remove(Provider provider)
        {
            _context.Providers.Remove(provider);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

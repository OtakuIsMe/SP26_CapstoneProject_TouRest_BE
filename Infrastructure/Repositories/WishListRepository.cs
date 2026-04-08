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
    public class WishListRepository : BaseRepository<Wishlist>, IWishListRepository
    {
        public WishListRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetDuplicateAsync(Guid userId, Guid itemId)
        {
            return await _context.Wishlists
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ItemId == itemId);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> ServiceExistsAsync(Guid serviceId)
        {
            return await _context.Services.AnyAsync(s => s.Id == serviceId);
        }

        public async Task<bool> PackageExistsAsync(Guid packageId)
        {
            return await _context.Packages.AnyAsync(p => p.Id == packageId);
        }
    }
}

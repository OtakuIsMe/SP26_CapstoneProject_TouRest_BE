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
        public async Task<List<Wishlist>> GetWishListsByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists.Where(w => w.UserId == userId).Include(x=>x.User).ToListAsync();
        }
        public async Task<Wishlist?> GetWishList(Guid id)
        {
            return await _context.Wishlists.Include(x => x.User).FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<List<Wishlist>> GetWishLists()
        {
            return await _context.Wishlists.Include(x => x.User).AsNoTracking().ToListAsync();
        }
         public async Task<List<Wishlist>> GetWishLists(WishListSearch search)
        {
            var query = _context.Wishlists.Include(x => x.User).AsNoTracking().AsQueryable();
            if(search.ItemType.HasValue)
            {
                query = query.Where(w => w.ItemType == search.ItemType.Value);
                if(search.ItemId.HasValue)
                {
                    query = query.Where(w => w.ItemId == search.ItemId.Value);
                }
            }
            return await query.ToListAsync();
        }
    }
}

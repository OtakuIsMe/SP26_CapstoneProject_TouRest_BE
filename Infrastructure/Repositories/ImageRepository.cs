using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ImageRepository : BaseRepository<Image>, IImageRepository
    {
        public ImageRepository(AppDbContext context) : base(context) { }

        public async Task<List<Image>> GetByTypeAsync(ImageType type, Guid typeId)
        {
            return await _context.Set<Image>()
                .AsNoTracking()
                .Where(i => i.Type == type && i.TypeId == typeId)
                .OrderBy(i => i.PicNumber)
                .ToListAsync();
        }
    }
}

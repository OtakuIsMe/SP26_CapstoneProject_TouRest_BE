using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IImageRepository : IBaseRepository<Image>
    {
        Task<List<Image>> GetByTypeAsync(ImageType type, Guid typeId);
    }
}

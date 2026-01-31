using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role?> GetByCodeAsync(string code);
    }
}

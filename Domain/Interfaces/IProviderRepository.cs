using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IProviderRepository
    {
        Task<List<Provider>> GetAllAsync();
        Task<(List<Provider> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<Provider?> GetByIdAsync(Guid id);
        Task<Provider?> GetByContactEmailAsync(string contactEmail);
        Task<Provider?> GetByCreateByUserIdAsync(Guid userId);
        Task<bool> ExistsByContactEmailAsync(string contactEmail);
        Task AddAsync(Provider provider);
        void Update(Provider provider);
        void Remove(Provider provider);
        Task SaveChangesAsync();
    }
}

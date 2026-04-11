using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Provider;

namespace TouRest.Application.Interfaces
{
    public interface IProviderService
    {
        Task<List<ProviderResponse>> GetAllAsync();
        Task<ProviderResponse?> GetByIdAsync(Guid id);
        Task<ProviderResponse> CreateAsync(Guid currentUserId, CreateProviderRequest request);
        Task<ProviderResponse?> UpdateAsync(Guid id, UpdateProviderRequest request);
        Task<bool> DeleteAsync(Guid id);
        //Task<ProviderResponse> CreateAsync(CreateProviderRequest request);
    }
}

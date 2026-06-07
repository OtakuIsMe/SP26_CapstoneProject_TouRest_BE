using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TouRest.Application.DTOs.Provider;
using TouRest.Domain.Enums;

namespace TouRest.Application.Interfaces
{
    public interface IProviderUserService
    {
        Task<ProviderStaffDTO> CreateStaffAccountAsync(Guid providerId, CreateProviderStaffRequest request);
        Task<List<ProviderStaffDTO>> GetStaffAsync(Guid providerId);
        Task RemoveStaffAsync(Guid providerId, Guid userId);
        Task AddUserToProviderAsync(Guid providerId, Guid userId, ProviderUserRole role);
        Task<ProviderStaffDTO?> GetProviderUserByUserId(Guid userId);
    }
}

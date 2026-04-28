using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IProviderUserRepository :IBaseRepository<ProviderUser>
    {
        Task AddUserIntoProvider(Guid providerId, Guid userId, ProviderUserRole role);
        Task<ProviderUser?> GetByUserIdAsync(Guid userId);
    }
}

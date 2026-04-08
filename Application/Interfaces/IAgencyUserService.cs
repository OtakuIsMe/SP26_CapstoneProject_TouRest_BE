using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyUserService
    {
        Task<bool> IsUserInAgencyAsync(Guid userId, Guid agencyId);
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId);
        Task<List<Guid>> GetAgenciesForUserAsync(Guid userId);
        Task<List<Guid>> GetUsersInAgencyAsync(Guid agencyId);
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
    }
}

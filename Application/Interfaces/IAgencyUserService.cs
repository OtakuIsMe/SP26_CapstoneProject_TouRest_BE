using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyUserService
    {
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId);
            Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
        Task<List<Guid>> GetUsersInAgencyAsync(Guid agencyId);
        Task<List<Guid>> GetAgenciesForUserAsync(Guid userId);
        Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId);
    }
}

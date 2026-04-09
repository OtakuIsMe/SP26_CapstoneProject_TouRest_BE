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
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyUserService
    {
        Task<bool> IsUserInAgencyAsync(Guid userId, Guid agencyId);
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId);
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
        Task<List<AgencyUserDTO>> GetAgencyUsers(Guid agencyId);
    }
}

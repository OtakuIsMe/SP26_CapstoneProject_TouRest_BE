using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyUserService : IAgencyUserService
    {
        public Task AddUserToAgencyAsync(Guid agencyId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Guid>> GetAgenciesForUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Guid>> GetUsersInAgencyAsync(Guid agencyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

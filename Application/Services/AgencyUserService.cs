using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyUserService : IAgencyUserService
    {
        private readonly IAgencyUserRepository _agencyRepository;
        public AgencyUserService(IAgencyUserRepository agencyRepository)
        {
            _agencyRepository = agencyRepository;
        }
        public async Task AddUserToAgencyAsync(Guid agencyId, Guid userId)
        {
            await _agencyRepository.AddUserToAgencyAsync(agencyId, userId);
        }

        public async Task<bool> IsUserInAgencyAsync(Guid agencyId, Guid userId)
        {
           return  await _agencyRepository.IsUserInAgencyAsync(userId, agencyId);
        }

        public async Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId)
        {
            await _agencyRepository.RemoveUserFromAgencyAsync(agencyId, userId);

        }
    }
}

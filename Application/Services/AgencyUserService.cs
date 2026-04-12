using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyUserService : IAgencyUserService
    {
        private readonly IAgencyUserRepository _agencyRepository;
        private readonly IMapper _mapper;
        public AgencyUserService(IAgencyUserRepository agencyRepository, IMapper mapper)
        {
            _agencyRepository = agencyRepository;
            _mapper = mapper;
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
        public async Task<List<AgencyUserDTO>> GetAgencyUsers(Guid agencyId)
        {
            return _mapper.Map<List<AgencyUserDTO>>(await _agencyRepository.GetAgencyUsers(agencyId));
        }
    }
}

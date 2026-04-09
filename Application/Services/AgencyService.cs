using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IMapper _mapper;   
        private readonly IAgencyRepository _agencyRepository;
        public AgencyService(IAgencyRepository agencyRepository, IMapper mapper)
        {
            _mapper = mapper;
            _agencyRepository = agencyRepository;
        }
        public async Task<AgencyDTO> AddAgency(AgencyCreateRequestDTO create)
        {
           var agency = _mapper.Map<Agency>(create);
            var createdAgency = await _agencyRepository.CreateAsync(agency);
            return _mapper.Map<AgencyDTO>(createdAgency);
        }

        public async Task<bool> DeleteAgency(Guid id)
        {
            return await _agencyRepository.DeleteAsync(id);
        }

        public async Task<AgencyDTO> GetAgencyById(Guid id)
        {
            var agency = await _agencyRepository.GetByIdAsync(id);
            return _mapper.Map<AgencyDTO>(agency);
        }

        public async Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update)
        {
            var existing = await _agencyRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Agency with ID {id} not found.");
            }
            _mapper.Map(update, existing);
            var updatedAgency = await _agencyRepository.UpdateAsync(existing);
            return _mapper.Map<AgencyDTO>(updatedAgency);
        }

        public async Task<List<AgencyUserDTO>> GetAgencyUsers(Guid agencyId)
        {
            return _mapper.Map<List<AgencyUserDTO>>(await _agencyRepository.GetAgencyUsers(agencyId));
        }
    }

}


using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Helpers;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
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
        public async Task<AgencyDTO> AddAgency(Guid userCreateId, AgencyCreateRequestDTO create)
        {
            ArgumentNullException.ThrowIfNull(create);
            var agency = _mapper.Map<Agency>(create);
            agency.Id = Guid.NewGuid();
            agency.CreateByUserId = userCreateId;

            var startTime = ParseTimeHelper.ParseTime(create.StartTime);
            var endTime = ParseTimeHelper.ParseTime(create.EndTime);

            if (startTime >= endTime)
                throw new ArgumentException("StartTime must be earlier than EndTime");

            agency.StartTime = startTime;
            agency.EndTime = endTime;
            agency.Status = AgencyStatus.Pending;
            agency.CreatedAt = DateTime.UtcNow;
            agency.UpdatedAt = DateTime.UtcNow;
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
        public async Task<AgencyDTO> GetMyAgency(Guid userId)
        {
            var agency = await _agencyRepository.GetMyAgency(userId);
            return _mapper.Map<AgencyDTO>(agency);
        }
        public async Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update)
        {
            var existing = await _agencyRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Agency with ID {id} not found.");
            }
            var newStart = existing.StartTime;
            var newEnd = existing.EndTime;

            if(update.StartTime != null)
                newStart = ParseTimeHelper.ParseTime(update.StartTime);
            if(update.EndTime != null)
                newEnd = ParseTimeHelper.ParseTime(update.EndTime);
            if (newStart >= newEnd)
                throw new Exception("StartTime must be earlier than EndTime");

            existing.StartTime = newStart;
            existing.EndTime = newEnd;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.Description = update.Description ?? existing.Description;
            var updatedAgency = await _agencyRepository.UpdateAsync(existing);
            return _mapper.Map<AgencyDTO>(updatedAgency);
        }


    }

}


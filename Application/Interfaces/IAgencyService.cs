using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyService
    {

        Task<AgencyDTO> GetAgencyById(Guid id);
        Task<AgencyDTO> GetMyAgency(Guid userId);
        Task<AgencyDTO> AddAgency(Guid userCreateId, AgencyCreateRequestDTO create);
        Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update);
        Task<bool> DeleteAgency(Guid id);
    }
}

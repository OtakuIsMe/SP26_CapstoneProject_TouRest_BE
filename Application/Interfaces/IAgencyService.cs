using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Admin;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IAgencyService
    {
        Task<List<AgencyUser>> GetAgencyUsers(Guid agencyId);
        Task<AgencyDTO> GetAgencyById(Guid id);
        Task<AgencyDTO> AddAgency(AgencyCreateRequestDTO create);
        Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update);
        Task<bool> DeleteAgency(Guid id);
    }
}

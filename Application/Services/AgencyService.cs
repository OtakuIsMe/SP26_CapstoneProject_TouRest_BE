using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;

namespace TouRest.Application.Services
{
    public class AgencyService : IAgencyService
    {
        public Task<AgencyDTO> AddAgency(AgencyCreateRequestDTO create)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAgency(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AgencyDTO> GetAgencyById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<AgencyDTO> UpdateAgency(Guid id, AgencyUpdateRequestDTO update)
        {
            throw new NotImplementedException();
        }

        public Task<List<AgencyDTO>> GetAgencyUsers(AgencySearchUser search)
        {
            return GetAgencyUsers(search);
        }
    }

}


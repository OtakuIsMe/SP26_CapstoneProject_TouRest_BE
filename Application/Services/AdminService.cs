using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Admin;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;

namespace TouRest.Application.Services
{
    public class AdminService : IAdminService
    {
        public Task ApproveAgency(Guid agencyId)
        {
            throw new NotImplementedException();
        }

        public Task ApproveProvider(Guid providerId)
        {
            throw new NotImplementedException();
        }

        public Task BanUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task DemoteFromAdminAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AgencyDTO>> GetAgencies(AgencySearch search)
        {
            throw new NotImplementedException();
        }

        public Task PromoteToAdminAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task RejectAgency(Guid agencyId)
        {
            throw new NotImplementedException();
        }

        public Task RejectProvider(Guid providerId)
        {
            throw new NotImplementedException();
        }

        public Task UnbanUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

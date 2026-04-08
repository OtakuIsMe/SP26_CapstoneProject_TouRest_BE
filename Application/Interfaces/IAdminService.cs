using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Admin;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Service;

namespace TouRest.Application.Interfaces
{
    public interface IAdminService
    {
        Task BanUserAsync(Guid userId);
        Task UnbanUserAsync(Guid userId);
        Task PromoteToAdminAsync(Guid userId);
        Task DemoteFromAdminAsync(Guid userId);
        Task ApproveAgency(Guid agencyId);
        Task RejectAgency(Guid agencyId);
        Task ApproveProvider(Guid providerId);
        Task RejectProvider(Guid providerId);
        Task<List<AgencyDTO>> GetAgencies(AgencySearch search);
    }
}

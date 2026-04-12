using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IAgencyUserRepository
    {
        Task<bool> IsUserInAgencyAsync(Guid userId, Guid agencyId);
        Task AddUserToAgencyAsync(Guid agencyId, Guid userId);
        Task RemoveUserFromAgencyAsync(Guid agencyId, Guid userId);
        Task<List<AgencyUser>> GetAgencyUsers(Guid agencyId);
        Task<AgencyUser?> GetAgencyUserByUserId(Guid userId);
        Task<List<AgencyUser>> SearchUsersByAgency(Guid id, SearchUserByAgency search);
    }
    public class SearchUserByAgency {
        public string? Email { get; set; }
        public string? FullName { get; set; }


    }

}

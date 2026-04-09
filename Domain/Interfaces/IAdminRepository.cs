using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IAdminRepository
    {
        Task BanUserAsync(Guid userId);
        Task UnbanUserAsync(Guid userId);
        Task PromoteToAdminAsync(Guid userId);
        Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status);
        Task ApproveAgency(Guid agencyId);
        Task RejectAgency(Guid agencyId);
        Task ApproveProvider(Guid providerId);
        Task RejectProvider(Guid providerId);
        Task<List<Agency>> GetAgencies(AgencySearch search);
        Task<List<Provider>> GetProviders(ProviderSearch search);
        Task<List<User>> GetUsers(UserSearch search);
    }
    public class AgencySearch
    {
        public string? AgencyName { get; set; }
        public AgencyStatus? Status { get; set; }
        public DateTime? MonthCreated { get; set; }
        public DateTime? DayCreated { get; set; }
    }
    public class ProviderSearch
    {
        public string? ProviderName { get; set; }
        public ProviderStatus? Status{ get; set; }
        public DateTime? MonthCreated { get; set; }
        public DateTime? DayCreated { get; set; }

    }
    public class UserSearch
    {
        public string? Email { get; set; }
        public UserStatus? Status { get; set; }
        public DateTime? MonthCreated { get; set; }
        public DateTime? DayCreated { get; set; }
    }
}

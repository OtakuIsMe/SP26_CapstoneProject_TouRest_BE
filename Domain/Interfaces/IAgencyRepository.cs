using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IAgencyRepository : IBaseRepository<Agency>
    {
        Task<Agency?> GetByContactEmailAsync(string contactEmail);
        Task<Agency?> GetMyAgency(Guid userId);
        Task<(List<Agency> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.PackageService;

namespace TouRest.Application.Interfaces
{
    public interface IPackageServiceService
    {
        Task<IEnumerable<PackageServiceSummaryDTO>> GetByPackageIdAsync(Guid packageId);
        Task<IEnumerable<PackageServiceSummaryDTO>> GetByServiceIdAsync(Guid serviceId);
        Task<PackageServiceDTO?> GetByIdsAsync(Guid packageId, Guid serviceId);
        Task<PackageServiceDTO> CreateAsync(Guid packageId, PackageServiceCreateRequest request);
        Task<PackageServiceDTO?> UpdateAsync(Guid packageId, Guid serviceId, PackageServiceUpdateRequest request);
        Task<bool> DeleteAsync(Guid packageId, Guid serviceId);
        Task<bool> IsServiceInPackageAsync(Guid packageId, Guid serviceId);
    }
}

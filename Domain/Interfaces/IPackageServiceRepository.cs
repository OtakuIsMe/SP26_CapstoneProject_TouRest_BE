using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IPackageServiceRepository
    {
        Task<List<PackageService>> GetPackageServicesByServiceId(Guid serviceId);
        Task<List<PackageService>> GetPackageServicesByPackageId(Guid packageId);
        Task<PackageService> GetPackageService(Guid packageId, Guid serviceId);
        Task<PackageService> UpdateAsync(PackageService packageService);
        Task<bool> DeleteAsync(Guid packageId, Guid serviceId);
        Task<PackageService> CreateAsync(PackageService packageService);
    }
}

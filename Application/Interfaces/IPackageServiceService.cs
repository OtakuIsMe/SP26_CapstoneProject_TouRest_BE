using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.Interfaces
{
    public interface IPackageServiceService
    {
        Task<List<TouRest.Domain.Entities.PackageService>> GetPackageServicesByServiceId(Guid serviceId);
        Task<List<TouRest.Domain.Entities.PackageService>> GetPackageServicesByPackageId(Guid packageId);
        Task<TouRest.Domain.Entities.PackageService> GetPackageService(Guid packageId, Guid serviceId);
        Task<TouRest.Domain.Entities.PackageService> UpdatePackageService(TouRest.Domain.Entities.PackageService packageService);
        Task<bool> DeletePackageService(Guid packageId, Guid serviceId);
        Task<TouRest.Domain.Entities.PackageService> CreatePackageService(TouRest.Domain.Entities.PackageService packageService);
        Task<bool> IsServiceInPackage(Guid packageId, Guid serviceId);
    }
}

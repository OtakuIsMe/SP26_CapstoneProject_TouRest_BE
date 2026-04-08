using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PackageServiceService : IPackageServiceService
    {
        private readonly IPackageServiceRepository _repository;
        public PackageServiceService(IPackageServiceRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<TouRest.Domain.Entities.PackageService>> GetPackageServicesByServiceId(Guid serviceId)
        {
            return await _repository.GetPackageServicesByServiceId(serviceId);
        }
        public async Task<List<TouRest.Domain.Entities.PackageService>> GetPackageServicesByPackageId(Guid packageId)
        {
            return await _repository.GetPackageServicesByPackageId(packageId);
        }
        public async Task<TouRest.Domain.Entities.PackageService> GetPackageService(Guid packageId, Guid serviceId)
        {
            return await _repository.GetPackageService(packageId, serviceId);
        }
        public async Task<TouRest.Domain.Entities.PackageService> UpdatePackageService(TouRest.Domain.Entities.PackageService packageService)
        {
            return await _repository.UpdateAsync(packageService);
        }
        public async Task<bool> DeletePackageService(Guid packageId, Guid serviceId)
        {
            return await _repository.DeleteAsync(packageId, serviceId);
        }
        public async Task<TouRest.Domain.Entities.PackageService> CreatePackageService(TouRest.Domain.Entities.PackageService packageService)
        {
            return await _repository.CreateAsync(packageService);
        }
        public async Task<bool> IsServiceInPackage(Guid packageId, Guid serviceId)
        {
            var packageService = await _repository.GetPackageService(packageId, serviceId);
            return packageService != null;
        }
    }
}

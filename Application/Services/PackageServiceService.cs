using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.PackageService;
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

        public async Task<IEnumerable<PackageServiceSummaryDTO>> GetByPackageIdAsync(Guid packageId)
        {
            var items = await _repository.GetPackageServicesByPackageId(packageId);
            return items.Select(MapToSummaryDTO);
        }

        public async Task<IEnumerable<PackageServiceSummaryDTO>> GetByServiceIdAsync(Guid serviceId)
        {
            var items = await _repository.GetPackageServicesByServiceId(serviceId);
            return items.Select(MapToSummaryDTO);
        }

        public async Task<PackageServiceDTO?> GetByIdsAsync(Guid packageId, Guid serviceId)
        {
            var item = await _repository.GetPackageService(packageId, serviceId);
            if (item == null) return null;

            return MapToDTO(item);
        }

        public async Task<PackageServiceDTO> CreateAsync(Guid packageId, PackageServiceCreateRequest request)
        {
            if (!await _repository.PackageExists(packageId))
                throw new KeyNotFoundException("Package not found.");

            if (!await _repository.ServiceExists(request.ServiceId))
                throw new KeyNotFoundException("Service not found.");

            var existed = await _repository.GetPackageService(packageId, request.ServiceId);
            if (existed != null)
                throw new InvalidOperationException("This service is already in the package.");

            var sortOrderExists = await _repository.ExistsSortOrderInPackage(packageId, request.SortOrder);
            if (sortOrderExists)
                throw new InvalidOperationException("SortOrder already exists in this package.");

            var entity = new TouRest.Domain.Entities.PackageService
            {
                PackageId = packageId,
                ServiceId = request.ServiceId,
                SortOrder = request.SortOrder
            };

            await _repository.CreateAsync(entity);

            var created = await _repository.GetPackageService(packageId, request.ServiceId);
            return MapToDTO(created!);
        }

        public async Task<PackageServiceDTO?> UpdateAsync(Guid packageId, Guid serviceId, PackageServiceUpdateRequest request)
        {
            var existing = await _repository.GetPackageService(packageId, serviceId);
            if (existing == null) return null;

            var sortOrderExists = await _repository.ExistsSortOrderInPackage(packageId, request.SortOrder, serviceId);
            if (sortOrderExists)
                throw new InvalidOperationException("SortOrder already exists in this package.");

            existing.SortOrder = request.SortOrder;

            var updated = await _repository.UpdateAsync(existing);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteAsync(Guid packageId, Guid serviceId)
        {
            return await _repository.DeleteAsync(packageId, serviceId);
        }

        public async Task<bool> IsServiceInPackageAsync(Guid packageId, Guid serviceId)
        {
            var item = await _repository.GetPackageService(packageId, serviceId);
            return item != null;
        }

        private static PackageServiceDTO MapToDTO(TouRest.Domain.Entities.PackageService ps)
        {
            return new PackageServiceDTO
            {
                PackageId = ps.PackageId,
                ServiceId = ps.ServiceId,
                SortOrder = ps.SortOrder,
                ServiceName = ps.Service?.Name ?? string.Empty,
                ServiceDescription = ps.Service?.Description,
                ServicePrice = ps.Service?.Price ?? 0,
                ServiceDurationMinutes = ps.Service?.DurationMinutes ?? 0,
                ServiceStatus = ps.Service!.Status,
                ServiceBasePrice = ps.Service?.BasePrice ?? 0
            };
        }

        private static PackageServiceSummaryDTO MapToSummaryDTO(TouRest.Domain.Entities.PackageService ps)
        {
            return new PackageServiceSummaryDTO
            {
                PackageId = ps.PackageId,
                ServiceId = ps.ServiceId,
                SortOrder = ps.SortOrder,
                ServiceName = ps.Service?.Name ?? string.Empty
            };
        }
    }
}

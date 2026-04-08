using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Package;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public async Task<IEnumerable<PackageSummaryDTO>> GetAllAsync()
        {
            var packages = await _packageRepository.GetAllAsync();
            return packages.Select(MapToSummaryDTO);
        }

        public async Task<PackageDTO?> GetByIdAsync(Guid id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null) return null;

            return MapToDTO(package);
        }

        public async Task<PackageDTO> CreateAsync(PackageCreateRequest request)
        {
            var existing = await _packageRepository.GetByCodeAsync(request.Code.Trim());
            if (existing != null)
                throw new InvalidOperationException("Package code already exists.");

            var package = new Package
            {
                Id = Guid.NewGuid(),
                Code = request.Code.Trim(),
                Name = request.Name.Trim(),
                BasePrice = request.BasePrice,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _packageRepository.CreateAsync(package);
            return MapToDTO(result);
        }

        public async Task<PackageDTO?> UpdateAsync(Guid id, PackageUpdateRequest request)
        {
            var existing = await _packageRepository.GetByIdAsync(id);
            if (existing == null) return null;

            var duplicate = await _packageRepository.GetByCodeAsync(request.Code.Trim());
            if (duplicate != null && duplicate.Id != id)
                throw new InvalidOperationException("Package code already exists.");

            existing.Code = request.Code.Trim();
            existing.Name = request.Name.Trim();
            existing.BasePrice = request.BasePrice;
            existing.Status = request.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _packageRepository.UpdateAsync(existing);
            return MapToDTO(result);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _packageRepository.DeleteAsync(id);
        }

        // ================= MAP =================

        private static PackageDTO MapToDTO(Package p)
        {
            return new PackageDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                BasePrice = p.BasePrice,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        private static PackageSummaryDTO MapToSummaryDTO(Package p)
        {
            return new PackageSummaryDTO
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                BasePrice = p.BasePrice,
                Status = p.Status
            };
        }
    }
}

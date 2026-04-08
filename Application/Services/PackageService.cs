using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<Package> GetPackageById(Guid id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null)
                throw new KeyNotFoundException("Package not found");
            return package;
        }
        public async Task<List<Package>> GetPackages()
        {
            var packages = await _packageRepository.GetAllAsync();
            return packages.ToList();
        }
        public async Task<Package> CreatePackage(Package package)
        {
            package.Id = Guid.NewGuid();
            var result = await _packageRepository.CreateAsync(package);
            return result;
        }
        public async Task<Package> UpdatePackage(Guid id, Package package)
        {
            var existing = await _packageRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Package not found");
            existing.Name = package.Name;
            var result = await _packageRepository.UpdateAsync(existing);
            return result;
        }
        public async Task<bool> DeletePackage(Guid id)
        {
            var result = await _packageRepository.DeleteAsync(id);
            return result;
        }
    }
    }

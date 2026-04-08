using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class PackageServiceRepository : IPackageServiceRepository
    {
        private readonly AppDbContext _context;
        public PackageServiceRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PackageService> CreateAsync(PackageService packageService)
        {
            _context.PackageServices.Add(packageService);
            await _context.SaveChangesAsync();
            return packageService;
        }
        public async Task<bool> DeleteAsync(Guid packageId, Guid serviceId)
        {
            var packageService = await _context.PackageServices.FindAsync(packageId, serviceId);
            if (packageService == null) return false;
            _context.PackageServices.Remove(packageService);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PackageService> UpdateAsync(PackageService packageService)
        {
            var existingPackageService = await _context.PackageServices.FindAsync(packageService.PackageId, packageService.ServiceId);
            if (existingPackageService == null) throw new Exception("PackageService not found");
            existingPackageService.SortOrder = packageService.SortOrder;
            await _context.SaveChangesAsync();
            return existingPackageService;

        }
        public async Task<PackageService> GetPackageService(Guid packageId, Guid serviceId)
        {
            var packageService = await _context.PackageServices.FindAsync(packageId, serviceId);
            if (packageService == null) throw new Exception("PackageService not found");
            return packageService;
        }
        public async Task<List<PackageService>> GetPackageServicesByPackageId(Guid packageId)
        {
            var packageServices = _context.PackageServices.Where(ps => ps.PackageId == packageId).ToList();
            return packageServices;
        }
        public async Task<List<PackageService>> GetPackageServicesByServiceId(Guid serviceId)
        {
            var packageServices = _context.PackageServices.Where(ps => ps.ServiceId == serviceId).ToList();
            return packageServices;
        }
        //public async Task<bool> IsServiceInPackage(Guid packageId, Guid serviceId)
        //{
        //    var packageService = await _context.PackageServices.FindAsync(packageId, serviceId);
        //    return packageService != null;
        //}
    }
}

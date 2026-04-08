using Microsoft.EntityFrameworkCore;
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
            if (existingPackageService == null)
                throw new KeyNotFoundException("PackageService not found");

            existingPackageService.SortOrder = packageService.SortOrder;
            await _context.SaveChangesAsync();

            return await _context.PackageServices
                .Include(ps => ps.Service)
                .FirstAsync(ps => ps.PackageId == packageService.PackageId && ps.ServiceId == packageService.ServiceId);
        }

        public async Task<PackageService?> GetPackageService(Guid packageId, Guid serviceId)
        {
            return await _context.PackageServices
                .Include(ps => ps.Service)
                .FirstOrDefaultAsync(ps => ps.PackageId == packageId && ps.ServiceId == serviceId);
        }

        public async Task<List<PackageService>> GetPackageServicesByPackageId(Guid packageId)
        {
            return await _context.PackageServices
                .Include(ps => ps.Service)
                .Where(ps => ps.PackageId == packageId)
                .OrderBy(ps => ps.SortOrder)
                .ToListAsync();
        }

        public async Task<List<PackageService>> GetPackageServicesByServiceId(Guid serviceId)
        {
            return await _context.PackageServices
                .Include(ps => ps.Service)
                .Where(ps => ps.ServiceId == serviceId)
                .OrderBy(ps => ps.SortOrder)
                .ToListAsync();
        }

        public async Task<bool> ExistsSortOrderInPackage(Guid packageId, int sortOrder, Guid? excludeServiceId = null)
        {
            return await _context.PackageServices.AnyAsync(ps =>
                ps.PackageId == packageId &&
                ps.SortOrder == sortOrder &&
                (!excludeServiceId.HasValue || ps.ServiceId != excludeServiceId.Value));
        }

        public async Task<bool> PackageExists(Guid packageId)
        {
            return await _context.Packages.AnyAsync(p => p.Id == packageId);
        }

        public async Task<bool> ServiceExists(Guid serviceId)
        {
            return await _context.Services.AnyAsync(s => s.Id == serviceId);
        }

        //public async Task<bool> IsServiceInPackage(Guid packageId, Guid serviceId)
        //{
        //    var packageService = await _context.PackageServices.FindAsync(packageId, serviceId);
        //    return packageService != null;
        //}
    }
}

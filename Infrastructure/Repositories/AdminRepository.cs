using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Constants;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TouRest.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task BanUserAsync(Guid userId)
        {
            await _context.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, UserStatus.Locked));
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            await _context.Users.Where(u => u.Id == userId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, UserStatus.Active));
        }

        public async Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status)
        {
            await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(u => u.RoleId, roleId)
                    .SetProperty(u => u.Status, status ?? UserStatus.Active)
                );
        }

        public async Task ApproveAgency(Guid agencyId)
        {
            await _context.Agencies.Where(a => a.Id == agencyId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, AgencyStatus.Active));
        }

        public async Task RejectAgency(Guid agencyId)
        {
            await _context.Agencies.Where(a => a.Id == agencyId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, AgencyStatus.Suspended));
        }

        public async Task ApproveProvider(Guid providerId)
        {
            await _context.Providers.Where(p => p.Id == providerId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, ProviderStatus.Active));
        }

        public async Task RejectProvider(Guid providerId)
        {
            await _context.Providers.Where(p => p.Id == providerId).ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, ProviderStatus.Suspended));
        }

        public async Task<List<Agency>> GetAgencies(AgencySearch search)
        {
            return await _context.Agencies.Where(a =>
                (string.IsNullOrEmpty(search.AgencyName) || a.Name.Contains(search.AgencyName)) &&
                (!search.Status.HasValue || a.Status == search.Status.Value) &&
                (!search.MonthCreated.HasValue || a.CreatedAt.Month == search.MonthCreated.Value.Month) &&
                (!search.DayCreated.HasValue || a.CreatedAt.Day == search.DayCreated.Value.Day)
            ).ToListAsync();
        }

        public Task<List<Provider>> GetProviders(ProviderSearch search)
        {
            return _context.Providers.Where(p =>
                (string.IsNullOrEmpty(search.ProviderName) || p.Name.Contains(search.ProviderName)) &&
                (!search.Status.HasValue || p.Status == search.Status.Value) &&
                (!search.MonthCreated.HasValue || p.CreatedAt.Month == search.MonthCreated.Value.Month) &&
                (!search.DayCreated.HasValue || p.CreatedAt.Day == search.DayCreated.Value.Day)
            ).ToListAsync();
        }

        public Task PromoteToAdminAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

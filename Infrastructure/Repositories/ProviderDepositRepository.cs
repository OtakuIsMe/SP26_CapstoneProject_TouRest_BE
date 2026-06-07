using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class ProviderDepositRepository : BaseRepository<ProviderDeposit>, IProviderDepositRepository
    {
        public ProviderDepositRepository(AppDbContext context) : base(context) { }

        public async Task<List<ProviderDeposit>> GetByScheduleIdAsync(Guid scheduleId)
            => await _context.ProviderDeposits
                .Where(d => d.ItineraryScheduleId == scheduleId)
                .ToListAsync();

        public async Task UpdateRangeAsync(IEnumerable<ProviderDeposit> deposits)
        {
            foreach (var d in deposits)
                d.UpdatedAt = DateTime.UtcNow;
            _context.ProviderDeposits.UpdateRange(deposits);
            await _context.SaveChangesAsync();
        }
    }
}

using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IProviderDepositRepository : IBaseRepository<ProviderDeposit>
    {
        Task<List<ProviderDeposit>> GetByScheduleIdAsync(Guid scheduleId);
        Task UpdateRangeAsync(IEnumerable<ProviderDeposit> deposits);
    }
}

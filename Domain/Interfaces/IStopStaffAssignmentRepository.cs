using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IStopStaffAssignmentRepository
    {
        Task UpsertAsync(Guid scheduleId, Guid stopId, Guid staffId);
        Task DeleteAsync(Guid scheduleId, Guid stopId);
        Task<List<StopStaffAssignment>> GetByScheduleIdAsync(Guid scheduleId);
        Task<List<StopStaffAssignment>> GetByScheduleIdsAsync(List<Guid> scheduleIds);
    }
}

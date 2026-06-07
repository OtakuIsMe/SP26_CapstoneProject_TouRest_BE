using Microsoft.EntityFrameworkCore;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class StopStaffAssignmentRepository : IStopStaffAssignmentRepository
    {
        private readonly AppDbContext _context;
        public StopStaffAssignmentRepository(AppDbContext context) => _context = context;

        public async Task UpsertAsync(Guid scheduleId, Guid stopId, Guid staffId)
        {
            var existing = await _context.StopStaffAssignments
                .FirstOrDefaultAsync(a => a.ScheduleId == scheduleId && a.StopId == stopId);

            if (existing != null)
            {
                existing.StaffId = staffId;
            }
            else
            {
                await _context.StopStaffAssignments.AddAsync(new StopStaffAssignment
                {
                    Id         = Guid.NewGuid(),
                    ScheduleId = scheduleId,
                    StopId     = stopId,
                    StaffId    = staffId,
                    CreatedAt  = DateTime.UtcNow,
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid scheduleId, Guid stopId)
        {
            var existing = await _context.StopStaffAssignments
                .FirstOrDefaultAsync(a => a.ScheduleId == scheduleId && a.StopId == stopId);
            if (existing != null)
            {
                _context.StopStaffAssignments.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<StopStaffAssignment>> GetByScheduleIdAsync(Guid scheduleId)
        {
            return await _context.StopStaffAssignments
                .Include(a => a.Staff)
                .Where(a => a.ScheduleId == scheduleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StopStaffAssignment>> GetByScheduleIdsAsync(List<Guid> scheduleIds)
        {
            return await _context.StopStaffAssignments
                .Include(a => a.Staff)
                .Where(a => scheduleIds.Contains(a.ScheduleId))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

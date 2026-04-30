using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryScheduleRepository : IBaseRepository<ItinerarySchedule>
    {
        Task<List<ItinerarySchedule>> GetByItineraryIdAsync(Guid itineraryId);
        Task<ItinerarySchedule?> GetByIdWithGuideAsync(Guid id);
    }
}

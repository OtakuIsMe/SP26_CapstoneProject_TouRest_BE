using TouRest.Application.DTOs.Itinerary;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryScheduleService
    {
        Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId);
        Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request);
        Task<bool> DeleteAsync(Guid scheduleId);
    }
}

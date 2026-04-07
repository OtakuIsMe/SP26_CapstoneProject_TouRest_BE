using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.ItineraryActivity;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryActivityService
    {
        Task<List<ItineraryActivityDTO>> GetActivitiesByItineraryStopId(Guid itineraryStopId);
        Task<ItineraryActivityDTO> GetItineraryActivity(Guid id);
        Task<ItineraryActivityDTO> AddItineraryActivity(ItineraryActivityCreateRequest create, Guid stopId);
        Task<bool> DeleteItineraryActivity(Guid id);
        Task<ItineraryActivityDTO> UpdateItineraryActivity(Guid id, ItineraryActivityUpdateRequest update);
    }
}

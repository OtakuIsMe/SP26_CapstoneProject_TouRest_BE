using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.ItineraryStop;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryStopService
    {
        Task<List<ItineraryStopDTO>> GetItineraryStopsByItineraryId(Guid itineraryId);
        Task<ItineraryStopDTO> AddItineraryStop(ItineraryStopCreateRequest create, Guid itineraryId);
        Task<ItineraryStopDTO> UpdateItineraryStop(Guid id, ItineraryStopUpdateRequest update);
        Task<bool> DeleteItineraryStop(Guid id);
        Task<ItineraryStopDTO?> GetItineraryStopById(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryService
    {
        Task<List<ItineraryDTO>> GetItineraries(ItinerarySearch search);
        Task<ItineraryDTO> AddItinerary(ItineraryCreateRequest create);
        Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateRequest update);
        Task<bool> DeleteItinerary(Guid id);
        Task<ItineraryDTO?> GetItineraryById(Guid id);
        Task<ItineraryDTO?> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest status);

    }
}

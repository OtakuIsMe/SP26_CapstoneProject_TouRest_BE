using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IItineraryService
    {
        Task<List<ItineraryDTO>> GetItineraries(ItinerarySearch search);
        Task<ItineraryDTO> AddItinerary(ItineraryCreateDTO create);
        Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateDTO update);
        Task<bool> DeleteItinerary(Guid id);
    }
}

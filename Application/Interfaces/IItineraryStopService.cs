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
        Task<ItineraryStopDTO> AddItineraryStop(ItineraryStopCreateDTO create);
    }
}

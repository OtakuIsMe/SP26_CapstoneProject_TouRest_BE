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
    }
}

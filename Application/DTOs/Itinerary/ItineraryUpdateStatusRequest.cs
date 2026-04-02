using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryUpdateStatusRequest
    {
        public ItineraryStatus Status { get; set; }
    }
}

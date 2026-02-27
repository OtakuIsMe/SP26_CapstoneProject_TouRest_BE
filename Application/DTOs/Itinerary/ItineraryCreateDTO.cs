using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int DurationDay { get; set; }
        public Guid AgencyId { get; set; }
    }
}

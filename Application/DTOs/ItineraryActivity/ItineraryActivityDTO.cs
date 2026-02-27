using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.ItineraryStop;

namespace TouRest.Application.DTOs.ItineraryActivity
{
    public class ItineraryActivityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Day { get; set; }
        public ItineraryStopSummaryDTO ItineraryStop { get; set; } = null!;
    }
}

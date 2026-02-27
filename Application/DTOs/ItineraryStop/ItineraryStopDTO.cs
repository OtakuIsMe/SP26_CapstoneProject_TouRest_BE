using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Itinerary;

namespace TouRest.Application.DTOs.ItineraryStop
{
    public class ItineraryStopDTO
    {
        public Guid Id { get; set; }
        public int StopOrder { get; set; }
        public string Name { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string? Address { get; set; }
        public ItinerarySummaryDTO Itinerary { get; set; } = null!;

    }
}

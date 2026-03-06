using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryUpdateRequest
    {
        public Guid AgencyId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int Price { get; set; }
        public int DurationDays { get; set; }
        public ItineraryStatus Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryRepository : IBaseRepository<Itinerary>
    {
        Task<List<Itinerary>> GetItineraries(ItinerarySearch search);
    }
    public class ItinerarySearch
    {
        public string? AgencyName { get; set; }
        public string? Name { get; set; }
        public long? LowPrice { get; set; }
        public long? HighPrice { get; set; }
        public int? LowDurationDay { get; set; }
        public int? HighDurationDay { get; set; }
        public string? Destination { get; set; }
        public VehicleType? VehicleType { get; set; }
    }
}

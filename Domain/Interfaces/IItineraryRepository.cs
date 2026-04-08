using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

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
        public int? LowPrice { get; set; }
        public int? HighPrice { get; set; }
        public int? LowDurationDay { get; set; }
        public int? HighDurationDay { get; set; }
    }
}

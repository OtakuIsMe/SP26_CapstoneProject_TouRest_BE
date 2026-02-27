using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IItineraryActivityRepository : IBaseRepository<ItineraryActivity>
    {
        Task<List<ItineraryActivity>> GetActivitiesByItineraryStopId(Guid itineraryStopId);
        Task<ItineraryActivity?> GetItineraryActivity(Guid id);
    }
    public class ItineraryActivitySearch
    { 
        public string? ItineraryStopName { get; set; }
        public string? ServiceName { get; set; }
        
        //[Range(0, int.MaxValue, ErrorMessage = "ActivityOrder must be greater than or equal to 0")]
        //public int? ActivityOrder { get; set; }      
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int? LowPrice { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int? HighPrice { get; set; }
    }
}

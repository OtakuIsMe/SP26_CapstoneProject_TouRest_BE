using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryActivity
{
    public class ItineraryActivityUpdateRequest
    {
        public Guid ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }
    }
}

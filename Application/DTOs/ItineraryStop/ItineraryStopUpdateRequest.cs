using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryStop
{
    public class ItineraryStopUpdateRequest
    {
        [Range(0, int.MaxValue, ErrorMessage = "StopOrder must be greater than or equal to 0")]
        public int StopOrder { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = null!;

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string? Address { get; set; }
        public Guid? ProviderId { get; set; }
    }
}

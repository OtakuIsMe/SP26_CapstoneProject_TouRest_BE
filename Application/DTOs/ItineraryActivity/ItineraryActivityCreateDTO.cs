using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryActivity
{
    public class ItineraryActivityCreateDTO
    {
        [Required]
        public Guid ItineraryStopId { get; set; }
        public Guid ServiceId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ActivityOrder must be greater than or equal to 0")]
        public int ActivityOrder { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }        
    }
}

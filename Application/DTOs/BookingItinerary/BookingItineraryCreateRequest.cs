using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.BookingItinerary
{
    public class BookingItineraryCreateRequest
    {
        [Required]
        public Guid BookingId { get; set; }
        [Required]
        public Guid ItineraryScheduleId { get; set; }
        public Guid? VoucherId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }
    }
}

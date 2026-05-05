using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.BookingItinerary
{
    public class BookingItineraryUpdateRequest
    {
        public Guid? VoucherId { get; set; }              
        public int? NumberOfGuests { get; set; }          
        public BookingItineraryStatus? Status { get; set; } 
    }
}

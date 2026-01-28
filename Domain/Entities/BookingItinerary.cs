using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("booking_itineraries")]
    public class BookingItinerary : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public Guid ItineraryId { get; set; }

        public Guid? VoucherId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public BookingItineraryStatus Status { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public Itinerary Itinerary { get; set; } = null!;
        public Voucher? Voucher { get; set; }
    }
}

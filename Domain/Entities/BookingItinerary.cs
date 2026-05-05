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
        public Guid ItineraryScheduleId { get; set; }

        public Guid? VoucherId { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public long Price { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public long FinalPrice { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        [Required]

?>":/     public BookingItineraryStatus Status { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public ItinerarySchedule ItinerarySchedule { get; set; } = null!;
        public Voucher? Voucher { get; set; }
        public Feedback? Feedback { get; set; }
    }
}

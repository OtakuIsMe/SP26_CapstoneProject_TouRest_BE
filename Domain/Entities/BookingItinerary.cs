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
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalPrice { get; set; }
        [Required]
        public BookingItineraryStatus Status { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public ItinerarySchedule ItinerarySchedule { get; set; } = null!;
        public Voucher? Voucher { get; set; }
    }
}

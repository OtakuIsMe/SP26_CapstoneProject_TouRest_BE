using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_tracking")]
    public class ItineraryTracking : BaseEntity
    {
        [Required]
        public Guid ItineraryScheduleId { get; set; }

        [Required]
        public Guid TrackingId { get; set; }

        [Required]
        public ItineraryTrackingType Type { get; set; }

        // Navigation properties
        public ItinerarySchedule ItinerarySchedule { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_activities")]
    public class ItineraryActivity : BaseEntity
    {
        [Required]
        public Guid ItineraryStopId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        public int ActivityOrder { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int Price { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        // Navigation properties
        public ItineraryStop ItineraryStop { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("itinerary_stops")]
    public class ItineraryStop : BaseEntity
    {
        [Required]
        public Guid ItineraryId { get; set; }

        [Required]
        public int StopOrder { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        // Navigation properties
        public Itinerary Itinerary { get; set; } = null!;
    }
}

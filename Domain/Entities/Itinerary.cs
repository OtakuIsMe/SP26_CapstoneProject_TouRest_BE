using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("itineraries")]
    public class Itinerary : BaseEntity
    {
        [Required]
        public Guid AgencyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int DurationDays { get; set; }

        [Required]
        public ItineraryStatus Status { get; set; }

        // Navigation properties
        public User Agency { get; set; } = null!;
    }
}

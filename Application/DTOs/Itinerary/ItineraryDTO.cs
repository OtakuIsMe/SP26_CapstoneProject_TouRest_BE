using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Itinerary
{
    public class ItineraryDTO
    {
        public Guid Id { get; set; }
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

    }
}

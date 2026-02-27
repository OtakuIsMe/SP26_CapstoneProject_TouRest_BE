using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.ItineraryActivity
{
    public class ItineraryActivitySummaryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

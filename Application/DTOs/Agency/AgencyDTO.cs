using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        public AgencyStatus Status { get; set; }
        public string Description { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string ContactEmail { get; set; } = null!;

        public string ContactPhone { get; set; } = null!;
        public Guid CreateByUserId { get; set; }
    }
}

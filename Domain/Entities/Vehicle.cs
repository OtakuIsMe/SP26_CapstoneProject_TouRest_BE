using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        [Range(1,100)]
        public int Capacity { get; set; }
        public VehicleType Type { get; set; }
        public Guid AgencyId { get; set; }
        public Agency Agency { get; set; } = null!;
    }
}

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
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        public AgencyStatus Status { get; set; }
        public string Description { get; set; } = null!;

        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}

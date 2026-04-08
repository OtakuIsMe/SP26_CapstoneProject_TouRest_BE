using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyCreateRequestDTO
    {
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}


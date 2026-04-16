using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyUserDTO
    {
        public Guid AgencyId { get; set; }
        public Guid UserId { get; set; }
        public string AgencyName { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}

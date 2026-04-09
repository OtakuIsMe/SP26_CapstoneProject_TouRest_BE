using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyUserDTO
    {
        public string AgencyName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool IsPrimaryContact { get; set; }
        public string Email { get; set; } = null!;
    }
}

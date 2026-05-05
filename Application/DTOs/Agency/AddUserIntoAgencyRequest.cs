using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Agency
{
    public class AddUserIntoAgencyRequest
    {
        [Required]
        public Guid AddUserId { get; set; }
        [Required]
        public AgencyUserRole Role { get; set; }
    }
}

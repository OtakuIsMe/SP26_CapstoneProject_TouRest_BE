using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyUpdateRequestDTO
    {
        public string? Description { get; set; }
        [RegularExpression(@"^\d{2}:\d{2}:\d{2}$")]
        public string? StartTime { get; set; }
        [RegularExpression(@"^\d{2}:\d{2}:\d{2}$")]
        public string? EndTime { get; set; }
    }
}

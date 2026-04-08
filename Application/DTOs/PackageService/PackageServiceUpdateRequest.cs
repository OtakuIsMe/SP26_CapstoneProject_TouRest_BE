using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.PackageService
{
    public class PackageServiceUpdateRequest
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "SortOrder must be greater than or equal to 0")]
        public int SortOrder { get; set; }
    }
}

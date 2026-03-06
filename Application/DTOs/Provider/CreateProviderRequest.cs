using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Provider
{
    public class CreateProviderRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}

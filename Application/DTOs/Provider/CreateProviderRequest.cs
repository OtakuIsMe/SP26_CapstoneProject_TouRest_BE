using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Provider
{
    //public class CreateProviderRequest
    //{
    //    [Required]
    //    [MaxLength(255)]
    //    public string Name { get; set; } = null!;

    //    [Required]
    //    [EmailAddress]
    //    [MaxLength(255)]
    //    public string ContactEmail { get; set; } = null!;

    //    [Required]
    //    [MaxLength(20)]
    //    public string ContactPhone { get; set; } = null!;
    //}

    public class CreateProviderRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ContactPhone { get; set; } = null!;
    }
}

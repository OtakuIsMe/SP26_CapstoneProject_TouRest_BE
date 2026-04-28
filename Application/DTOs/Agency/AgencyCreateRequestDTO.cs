using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TouRest.Application.DTOs.Agency
{
    public class AgencyCreateRequestDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [MaxLength(1000)]
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string StartTime { get; set; } = null!;
        [Required]
        public string EndTime { get; set; } = null!;
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string ContactEmail { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string ContactPhone { get; set; } = null!;
        public List<IFormFile>? Images { get; set; }
    }
}


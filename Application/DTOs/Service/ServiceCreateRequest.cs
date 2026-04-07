using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Service
{
    public class ServiceCreateRequest
    {
        [Required]
        public Guid ProviderId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "DurationMinutes must be greater than 0")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public int BasePrice { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Package
{
    public class PackageCreateRequest
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BasePrice must be greater than 0")]
        public int BasePrice { get; set; }

        [MinLength(1, ErrorMessage = "At least one service is required.")]
        public List<Guid> ServiceIds { get; set; } = [];
    }
}

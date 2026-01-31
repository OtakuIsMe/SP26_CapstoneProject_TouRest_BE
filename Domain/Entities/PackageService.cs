using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TouRest.Domain.Entities
{
    [Table("package_services")]
    public class PackageService
    {
        [Required]
        public Guid PackageId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "SortOrder must be greater than or equal to 0")]
        public int SortOrder { get; set; }

        // Navigation properties
        public Package Package { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}

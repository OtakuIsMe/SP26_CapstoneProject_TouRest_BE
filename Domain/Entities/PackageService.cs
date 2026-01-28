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
        public int SortOrder { get; set; }

        // Navigation properties
        public Package Package { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}

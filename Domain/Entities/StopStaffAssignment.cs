using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;

namespace TouRest.Domain.Entities
{
    [Table("stop_staff_assignments")]
    public class StopStaffAssignment : BaseEntity
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid StopId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        // Navigation properties
        public ItinerarySchedule Schedule { get; set; } = null!;
        public ItineraryStop Stop { get; set; } = null!;
        public User Staff { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Booking
{
    public class BookingCreateRequest
    {
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "NumberOfPeople must be at least 1")]
        public int NumberOfPeople { get; set; }

        public string? VoucherCode { get; set; }

        [MaxLength(500)]
        public string? CustomerNote { get; set; }
    }
}

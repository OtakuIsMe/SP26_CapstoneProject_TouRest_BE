using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Booking
{
    public class BookingCreateRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalAmount must be greater than 0")]
        public int TotalAmount { get; set; }

        [MaxLength(500)]
        public string? CustomerNote { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Refund
{
    public class RefundRequestDTO
    {
        [Required]
        public Guid BookingId { get; set; }
        [MaxLength(500)]
        public string? Reason { get; set; }
        [Required]
        public string CustomerBankAccount { get; set; } = null!;
        [Required]
        public string CustomerBankName { get; set; } = null!;
        [Required]
        public string CustomerAccountHolder { get; set; } = null!;
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.Refund
{
    public class RefundReviewDTO
    {
        [Required]
        public bool Approved { get; set; }
        public string? AdminNote { get; set; }
    }
}

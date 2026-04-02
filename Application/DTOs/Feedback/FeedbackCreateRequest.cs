using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Feedback
{
    public class FeedbackCreateRequest
    {
        [Required]
        public Guid BookingItineraryId { get; set; }

        [Required]
        public FeedbackItemType ItemType { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Required]
        public bool IsAnonymous { get; set; } = false;
    }
}

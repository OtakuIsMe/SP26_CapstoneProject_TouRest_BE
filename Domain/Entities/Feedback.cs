using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    [Table("feedbacks")]
    public class Feedback : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

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

        [Required]
        public FeedbackStatus Status { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = null!;
    }
}

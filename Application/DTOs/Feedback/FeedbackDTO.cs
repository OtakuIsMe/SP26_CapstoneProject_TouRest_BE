using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Feedback
{
    public class FeedbackDTO
    {
        public Guid BookingId { get; set; }
        public FeedbackItemType ItemType { get; set; }
        public Guid ItemId { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; } = null!;
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; } 
        public FeedbackStatus Status { get; set; }
    }
}

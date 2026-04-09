using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<FeedbackDTO>> GetFeedbacks();
        Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search);
        Task<List<FeedbackDTO>> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId);
        Task<FeedbackDTO?> GetFeedback(Guid id);
        Task<FeedbackDTO> AddFeedback(FeedbackCreateRequest create);
        Task<FeedbackDTO> UpdateFeedback(Guid id, FeedbackUpdateRequest update);
        Task<bool> DeleteFeedback(Guid id);
    }
}

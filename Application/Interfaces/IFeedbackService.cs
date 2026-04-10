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
        Task<FeedbackDTO> GetFeedback(Guid id);
        Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search);
        Task<List<FeedbackDTO>> GetFeedbacks();
        Task<FeedbackDTO> AddFeedback(FeedbackCreateRequest create);
        Task<bool> DeleteFeedback(Guid id);
        Task<FeedbackDTO> UpdateFeedback(Guid id, FeedbackUpdateRequest update);
        Task<List<FeedbackDTO>> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId);
    }
}

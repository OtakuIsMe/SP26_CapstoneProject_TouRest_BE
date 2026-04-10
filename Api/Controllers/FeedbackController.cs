using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFeedback(Guid id)
        {
            var feedback = await _feedbackService.GetFeedback(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(feedback);
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetFeedbacks([FromQuery] FeedbackSearch search)
        {
            var feedbacks = await _feedbackService.GetFeedbacks(search);
            return ApiResponseFactory.Ok(feedbacks);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] FeedbackUpdateRequest update)
        {
            var feedback = await _feedbackService.UpdateFeedback(id, update);
            return ApiResponseFactory.Ok(feedback);
        }
        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackCreateRequest create)
        {
            var feedback = await _feedbackService.AddFeedback(create);
            return ApiResponseFactory.Ok(feedback);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var result = await _feedbackService.DeleteFeedback(id);
            if (!result)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(result);
        }
        [HttpGet("bookingItinerary/{bookingItineraryId:guid}")]
        public async Task<IActionResult> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId)
        {
            var feedbacks = await _feedbackService.GetFeedbacksByBookingItineraryId(bookingItineraryId);
            return ApiResponseFactory.Ok(feedbacks);
        }
        
    }

    }

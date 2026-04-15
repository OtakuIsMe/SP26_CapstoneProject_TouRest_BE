using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.Feedback;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
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

        // GET: api/feedback/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var result = await _feedbackService.GetFeedbacks();
            return Ok(result);
        }

        // GET: api/feedback/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetFeedback(Guid id)
        {
            var result = await _feedbackService.GetFeedback(id);
            if (result == null)
            {
                return NotFound(new { message = "Feedback not found" });
            }

            return Ok(result);
        }

        // POST: api/feedback
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _feedbackService.AddFeedback(request);
            return CreatedAtAction(nameof(GetFeedback), new { id = result.BookingItineraryId }, result);
        }

        // PUT: api/feedback/{id}
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] FeedbackUpdateRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _feedbackService.UpdateFeedback(id, update);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/feedback/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var deleted = await _feedbackService.DeleteFeedback(id);
            if (!deleted)
            {
                return NotFound(new { message = "Feedback not found" });
            }

            return Ok(new { message = "Feedback deleted successfully" });
        }

        //// GET: api/feedback/booking-itinerary/{bookingItineraryId}
        //[HttpGet("booking-itinerary/{bookingItineraryId:guid}")]
        //public async Task<IActionResult> GetFeedbacksByBookingItineraryId(Guid bookingItineraryId)
        //{
        //    var result = await _feedbackService.GetFeedbacksByBookingItineraryId(bookingItineraryId);
        //    return Ok(result);
        //}

        // GET: api/feedback
        [HttpGet("search")]
        public async Task<IActionResult> GetFeedbacks(
            [FromQuery] string? bookingCode,
            [FromQuery] FeedbackItemType? itemType,
            [FromQuery] int? rating,
            [FromQuery] string? title,
            [FromQuery] bool? isAnonymous,
            [FromQuery] FeedbackStatus? status)
        {
            var search = new FeedbackSearch
            {
                BookingCode = bookingCode,
                ItemType = itemType,
                Rating = rating,
                Title = title,
                IsAnonymous = isAnonymous,
                Status = status
            };

            var result = await _feedbackService.GetFeedbacks(search);
            return Ok(result);
        }
    }
}

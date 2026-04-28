using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.BookingItinerary;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/booking-itineraries")]
    [ApiController]
    public class BookingItineraryController : ControllerBase
    {
        private readonly IBookingItineraryService _bookingItineraryService;
        public BookingItineraryController(IBookingItineraryService bookingItineraryService)
        {
            _bookingItineraryService = bookingItineraryService;
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetBookingItinerary(Guid id)
        {
            var result = await _bookingItineraryService.GetBookingItinerary(id);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("booking/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByBookingId(Guid bookingId)
        {
            var result = await _bookingItineraryService.GetBookingItinerariesByBookingId(bookingId);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateBookingItinerary([FromBody] BookingItineraryCreateRequest request)
        {
            var userId = User.GetUserId();
            var result = await _bookingItineraryService.CreateBookingItinerary(userId, request);
            return ApiResponseFactory.Created(result, "Booking itinerary created");
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "CUSTOMER, ADMIN")]
        public async Task<IActionResult> UpdateBookingItinerary(Guid id, [FromBody] BookingItineraryUpdateRequest update)
        {
            var userId = User.GetUserId();
            var result = await _bookingItineraryService.UpdateBookingItinerary(id, userId, update);
            return ApiResponseFactory.Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "CUSTOMER, ADMIN")]
        public async Task<IActionResult> DeleteBookingItinerary(Guid id)
        {
            var userId = User.GetUserId();
            await _bookingItineraryService.DeleteBookingItinerary(id, userId);
            return ApiResponseFactory.NoContent("Booking itinerary cancelled");
        }
    }
}

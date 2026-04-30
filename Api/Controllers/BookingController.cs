using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _bookingService.GetBookingAsync(id);
            return ApiResponseFactory.Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingCreateRequest request)
        {
            var userId = User.GetUserId();
            var result = await _bookingService.CreateBookingAsync(request, userId);
            return ApiResponseFactory.Created(result, "Booking created successfully");
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingUpdateRequest update)
        {
            var booking = await _bookingService.UpdateBookingAsync(id, update);
            return ApiResponseFactory.Ok(booking);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);
            if (!result) return NotFound();
            return ApiResponseFactory.NoContent();
        }
    }
}

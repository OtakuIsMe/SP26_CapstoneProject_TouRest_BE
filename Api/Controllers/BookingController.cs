using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Booking;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _bookingService.GetBookingAsync(id);
            return ApiResponseFactory.Ok(booking);
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public async Task<IActionResult> AddBooking([FromBody] BookingCreateRequest create)
        {
            var userId = User.GetUserId();
            await _bookingService.CreateBookingAsync(create, userId);
            return ApiResponseFactory.Created( new {}, "Booking was created");
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingUpdateRequest update)
        {
            var booking = await _bookingService.UpdateBookingAsync(id, update);
            return ApiResponseFactory.Ok(booking);
        }
         [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN, CUSTOMER")]
         public async Task<IActionResult> DeleteBooking(Guid id)
            {
            await _bookingService.DeleteBookingAsync(id);
            return ApiResponseFactory.Ok(new { }, "Booking deleted");
        }
       
    }
}
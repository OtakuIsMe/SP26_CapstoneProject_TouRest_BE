using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
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
            if (booking == null)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(booking);
        }
        //    [HttpGet("/search")]
        //public async Task<IActionResult> GetBookings([FromQuery] BookingSearch search)
        //{

        //}
        [HttpPost]
        [Authorize(Roles = "ADMIN, USER")]
        public async Task<IActionResult> AddBooking([FromBody] BookingCreateRequest create)
        {
            await _bookingService.CreateBookingAsync(create);
            return ApiResponseFactory.Created( new {}, "Booking was created");
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN, USER")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingUpdateRequest update)
        {
            var booking = await _bookingService.UpdateBookingAsync(id, update);
            if (booking == null)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(booking);
        }
         [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN, USER")]
         public async Task<IActionResult> DeleteBooking(Guid id)
            {
                var result = await _bookingService.DeleteBookingAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return ApiResponseFactory.Ok(result);
        }
       
    }
}
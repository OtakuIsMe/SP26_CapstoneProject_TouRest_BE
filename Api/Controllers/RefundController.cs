using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Refund;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/refunds")]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RequestRefund([FromBody] RefundRequestDTO request)
        {
            var userId = User.GetUserId();
            var refund = await _refundService.RequestRefundAsync(request, userId);
            return ApiResponseFactory.Created(refund, "Refund requested");
        }

        [HttpPut("{refundId:guid}/review")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ReviewRefund(Guid refundId, [FromBody] RefundReviewDTO review)
        {
            var refund = await _refundService.ReviewRefundAsync(refundId, review);
            return ApiResponseFactory.Ok(refund, "Refund reviewed");
        }

        [HttpPut("{refundId:guid}/complete")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CompleteRefund(Guid refundId)
        {
            var refund = await _refundService.CompleteRefundAsync(refundId);
            return ApiResponseFactory.Ok(refund, "Refund completed");
        }

        [HttpGet("booking/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetRefundByBooking(Guid bookingId)
        {
            var refund = await _refundService.GetRefundByBookingAsync(bookingId);
            return ApiResponseFactory.Ok(refund);
        }
    }
}

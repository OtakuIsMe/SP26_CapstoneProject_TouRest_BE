using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.DTOs.Payment;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnPayService;

        public PaymentController(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        /// <summary>
        /// Tạo QR thanh toán VNPay cho một booking.
        /// </summary>
        [HttpPost("qr")]
        public async Task<IActionResult> CreateQr([FromBody] CreateQrPaymentRequest request)
        {
            var ipAddr = HttpContext.Connection.RemoteIpAddress?.ToString()
                         ?? request.IpAddr
                         ?? "127.0.0.1";

            var result = await _vnPayService.GenerateQrAsync(
                request.BookingId,
                ipAddr,
                request.ExpireMinutes);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Message, code = result.Code });

            return ApiResponseFactory.Ok(result);
        }
    }
}

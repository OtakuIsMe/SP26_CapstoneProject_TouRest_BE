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
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IVNPayService vnPayService, IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _paymentService = paymentService;
            _logger = logger;
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

        [HttpPost("booking/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> CreatePayment(Guid bookingId)
        {
            var userId = User.GetUserId();
            var payment = await _paymentService.CreatePaymentAsync(bookingId, userId);
            return ApiResponseFactory.Ok(payment, "Payment link created");
        }

        [HttpDelete("booking/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> CancelPayment(Guid bookingId)
        {
            var userId = User.GetUserId();
            var payment = await _paymentService.CancelPaymentAsync(bookingId, userId);
            return ApiResponseFactory.Ok(payment, "Payment cancelled");
        }

        [HttpGet("booking/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetActivePayment(Guid bookingId)
        {
            var payment = await _paymentService.GetActivePaymentAsync(bookingId);
            return ApiResponseFactory.Ok(payment, "Active payment retrieved");
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook([FromBody] Webhook webhookData)
        {
            try
            {
                _logger.LogInformation("PayOS webhook received: {OrderCode}", webhookData.Data?.OrderCode);
                await _paymentService.HandleWebhookAsync(webhookData);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS webhook processing failed");
                return Ok(new { success = false }); 
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.Webhooks;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
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

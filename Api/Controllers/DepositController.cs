using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/deposits")]
    [Authorize]
    public class DepositController : ControllerBase
    {
        private readonly IDepositService _depositService;

        public DepositController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        /// <summary>
        /// Preview deposit required before creating a schedule.
        /// GET /api/deposits/calculate?itineraryId={id}&scheduleStart={iso}
        /// </summary>
        [HttpGet("calculate")]
        public async Task<IActionResult> Calculate([FromQuery] Guid itineraryId, [FromQuery] DateTime scheduleStart)
        {
            var result = await _depositService.CalculateAsync(itineraryId, scheduleStart);
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>Get deposit breakdown for an existing schedule.</summary>
        [HttpGet("schedules/{scheduleId:guid}")]
        public async Task<IActionResult> GetBySchedule(Guid scheduleId)
        {
            var result = await _depositService.GetByScheduleAsync(scheduleId);
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>
        /// Preview refund/forfeit outcome if agency cancels this schedule NOW.
        /// Call before confirming cancel to show agency the breakdown.
        /// </summary>
        [HttpGet("schedules/{scheduleId:guid}/cancel-preview")]
        public async Task<IActionResult> CancelPreview(Guid scheduleId)
        {
            var result = await _depositService.PreviewCancelAsync(scheduleId);
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>
        /// Agency cancels a Pending/Confirmed schedule. Applies 48h rule per provider:
        /// - cancelled &gt;48h before service → deposit refunded to agency wallet
        /// - cancelled ≤48h before service → deposit forfeited (admin keeps)
        /// Also cancels all customer bookings for the schedule.
        /// </summary>
        [HttpPost("schedules/{scheduleId:guid}/cancel")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CancelSchedule(Guid scheduleId)
        {
            try
            {
                var result = await _depositService.AgencyCancelScheduleAsync(scheduleId);
                return ApiResponseFactory.Ok(result, "Schedule cancelled");
            }
            catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Preview what will happen if agency cancels an ONGOING schedule now.
        /// Shows: completion ratio, agency earnings, deposit outcomes, customer refund amounts.
        /// </summary>
        [HttpGet("schedules/{scheduleId:guid}/cancel-ongoing-preview")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CancelOngoingPreview(Guid scheduleId)
        {
            try
            {
                var result = await _depositService.PreviewOngoingCancelAsync(scheduleId);
                return ApiResponseFactory.Ok(result);
            }
            catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Agency cancels an ONGOING schedule.
        /// - Agency receives: completed services value (tour price × ratio × pax) + deposits returned/refunded
        /// - Provider receives: deposit for upcoming services ≤48h (cancellation penalty)
        /// - Customer receives: amount paid − used services value
        /// </summary>
        [HttpPost("schedules/{scheduleId:guid}/cancel-ongoing")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CancelOngoingSchedule(Guid scheduleId)
        {
            try
            {
                var result = await _depositService.AgencyCancelOngoingScheduleAsync(scheduleId);
                return ApiResponseFactory.Ok(result, "Ongoing schedule cancelled");
            }
            catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Deposit;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.Payout;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;
using TouRest.Domain.DTOs;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/admins")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        private readonly IAgencyService _agencyService;
        private readonly IAdminDashboardService _dashboardService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IWalletService _walletService;
        private readonly IItineraryScheduleService _scheduleService;
        private readonly IDepositService _depositService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService, IAuthService authService,
            IAgencyService agencyService, IUserService userService, IEmailService emailService, IAdminDashboardService dashboardService, IWalletService walletService,
            IItineraryScheduleService scheduleService, IDepositService depositService)

        {
            _logger = logger;
            _adminService = adminService;
            _authService = authService;
            _agencyService = agencyService;
            _userService = userService;
            _emailService = emailService;
            _dashboardService = dashboardService;
            _walletService = walletService;
            _scheduleService = scheduleService;
            _depositService = depositService;
        }
        //agency
        [HttpGet("agencies/search")]
        
        public async Task<IActionResult> SearchAgencies([FromQuery] AgencySearch search)
        {
            var result = await _adminService.GetAgencies(search);
            return ApiResponseFactory.Ok(result);
        }
        [HttpGet("pending-agencies")]
        
        public async Task<IActionResult> GetPendingAgencies()
        {
            var search = new AgencySearch { Status = AgencyStatus.Pending };
            var result = await _adminService.GetAgencies(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPut("agencies/{id:guid}/approve")]
        
        public async Task<IActionResult> ApproveAgency(Guid id, [FromBody] CreateAgencyAccountRequest createAccount)
        {
            if (createAccount == null)
                throw new ArgumentNullException(nameof(createAccount), "Account details are required.");

            var userId = User.GetUserId();
            //_logger.LogInformation("Admin {AdminId} is approving agency {AgencyId}", userId, id);

            var agencyWithCreator = await _agencyService.GetAgencyByIdWithCreator(id);
            if (agencyWithCreator == null)
                throw new KeyNotFoundException("Agency not found");

            var email = agencyWithCreator.User?.Email;
            if (string.IsNullOrEmpty(email))
                throw new InvalidOperationException("Agency creator email is missing.");
            await _adminService.ApproveAgency(id);
            await _adminService.CreateAgencyAccount(id, createAccount);

       //     try
       //     {
       //         await _emailService.SendAsync(
       //             email,
       // "Your Agency Has Been Approved — Account Details",
       // $@"<h1>Congratulations!</h1>
       //<p>Your agency <strong>{agency.Name}</strong> has been approved.</p>
       //<h3>Your login credentials:</h3>
       //<p>Email: <strong>{createAccount.Email}</strong></p>
       //<p>Password: <strong>{createAccount.Password}</strong></p>
       //<p>Please log in and change your password immediately.</p>");
       //     }
       //     catch (Exception ex)
       //     {
       //         _logger.LogError(ex, "Failed to send approval email to {Email}", email);
       //     }
            return ApiResponseFactory.Ok(new { }, "Agency approved successfully");
        }

        [HttpPost("agencies/{id:guid}/create-account")]
        
        public async Task<IActionResult> CreateAgencyAccount(Guid id, [FromBody] CreateAgencyAccountRequest request)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is creating account for agency {AgencyId}", userId, id);

            await _adminService.CreateAgencyAccount(id, request);
            return ApiResponseFactory.Created(new { }, "Agency account created successfully");
        }
        [HttpPut("agencies/{id:guid}/reject")]
        
        public async Task<IActionResult> RejectAgency(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is rejecting agency {AgencyId}", userId, id);

            await _adminService.RejectAgency(id);
            return ApiResponseFactory.Ok(new { }, "Agency rejected successfully");
        }
        //provider
        [HttpPut("providers/{id:guid}/approve")]
        
        public async Task<IActionResult> ApproveProvider(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is approving provider {ProviderId}", userId, id);

            await _adminService.ApproveProvider(id);
            return ApiResponseFactory.Ok(new { }, "Provider approved successfully");
        }

        [HttpPut("providers/{id:guid}/reject")]
        
        public async Task<IActionResult> RejectProvider(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is rejecting provider {ProviderId}", userId, id);

            await _adminService.RejectProvider(id);
            return ApiResponseFactory.Ok(new { }, "Provider rejected successfully");
        }

        [HttpPost("providers/{id:guid}/create-account")]
        
        public async Task<IActionResult> CreateProviderAccount(Guid id, [FromBody] CreateProviderAccountRequest request)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is creating account for provider {ProviderId}", userId, id);

            await _adminService.CreateProviderAccount(id, request);
            return ApiResponseFactory.Created(new { }, "Provider account created successfully");
        }

        [HttpGet("pending-providers")]
        
        public async Task<IActionResult> GetPendingProviders()
        {
            var search = new ProviderSearch { Status = ProviderStatus.Pending };
            var result = await _adminService.GetProviders(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("providers/search")]
        
        public async Task<IActionResult> SearchProviders([FromQuery] ProviderSearch search)
        {
            var result = await _adminService.GetProviders(search);
            return ApiResponseFactory.Ok(result);
        }
        //user
        [HttpGet("users/search")]
        
        public async Task<IActionResult> SearchUsers([FromQuery] UserSearch search)
        {
            var result = await _adminService.GetUsers(search);
            return ApiResponseFactory.Ok(result);
        }
        [HttpPut("users/{id:guid}/ban")]
        
        public async Task<IActionResult> BanUser(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is banning user {UserId}", userId, id);

            await _adminService.BanUserAsync(id);
            return ApiResponseFactory.Ok(new { }, "User banned successfully");
        }

        [HttpPut("users/{id:guid}/unban")]
        
        public async Task<IActionResult> UnbanUser(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is unbanning user {UserId}", userId, id);

            await _adminService.UnbanUserAsync(id);
            return ApiResponseFactory.Ok(new { }, "User unbanned successfully");
        }

        //feedback

        [HttpGet("feedbacks")]
        
        public async Task<IActionResult> GetFeedbacks([FromQuery] FeedbackSearch search)
        {
            var result = await _adminService.GetFeedbacks(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpPut("feedbacks/{id:guid}/hide")]
        
        public async Task<IActionResult> HideFeedback(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is hiding feedback {FeedbackId}", userId, id);
            await _adminService.HideFeedback(id);
            return ApiResponseFactory.Ok(new { }, "Feedback hidden successfully");
        }

        [HttpDelete("feedbacks/{id:guid}")]
        
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is deleting feedback {FeedbackId}", userId, id);
            await _adminService.DeleteFeedback(id);
            return ApiResponseFactory.NoContent("Feedback deleted");
        }
        //Dashboard
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            return ApiResponseFactory.Ok(await _dashboardService.GetStatsAsync());
        }

        [HttpGet("bookings/trend")]
        public async Task<IActionResult> GetTrend([FromQuery] int year = 2026)
        {
            return ApiResponseFactory.Ok(await _dashboardService.GetTrendAsync(year));
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            return ApiResponseFactory.Ok(await _dashboardService.GetPendingApprovalsAsync());
        }

        [HttpGet("top-agencies")]
        public async Task<IActionResult> GetTopAgencies([FromQuery] int limit = 5)
        {
            return ApiResponseFactory.Ok(await _dashboardService.GetTopAgenciesAsync(limit));
        }
        [HttpGet("schedules")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var result = await _scheduleService.GetAllSchedulesAsync();
            return ApiResponseFactory.Ok(result);
        }

        /// <summary>
        /// Preview full refund outcome if admin cancels this schedule NOW.
        /// Admin always gets full deposit refund (no 48h rule) and customers get full trip refund.
        /// </summary>
        [HttpGet("schedules/{scheduleId:guid}/cancel-preview")]
        public async Task<IActionResult> PreviewCancelSchedule(Guid scheduleId)
        {
            try
            {
                var result = await _depositService.PreviewAdminCancelAsync(scheduleId);
                return ApiResponseFactory.Ok(result);
            }
            catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Admin cancels a schedule. Refunds ALL provider deposits to agency and full trip cost to customers.
        /// </summary>
        [HttpPost("schedules/{scheduleId:guid}/cancel")]
        public async Task<IActionResult> CancelSchedule(Guid scheduleId)
        {
            try
            {
                var result = await _depositService.AdminCancelScheduleAsync(scheduleId);
                return ApiResponseFactory.Ok(result, "Schedule cancelled");
            }
            catch (KeyNotFoundException ex)      { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        //payouts page
        [HttpGet("payouts")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var result = await _walletService.GetPendingPayoutsAsync();
            return ApiResponseFactory.Ok(result);
        }

        [HttpPut("payouts/{id:guid}/approve")]
        public async Task<IActionResult> ApprovePayout(Guid id, [FromBody] ApprovePayoutRequest request)
        {
            var adminId = User.GetUserId();
            await _walletService.ApprovePayoutAsync(id, adminId, request.Note);
            return ApiResponseFactory.Ok(new { }, "Payout approved — please transfer manually");
        }

        [HttpPut("payouts/{id:guid}/complete")]
        public async Task<IActionResult> CompletePayout(Guid id, [FromBody] CompletePayoutRequest request)
        {
            var adminId = User.GetUserId();
            await _walletService.CompletePayoutAsync(id, adminId, request.TransferReference);
            return ApiResponseFactory.Ok(new { }, "Payout marked as completed");
        }

        [HttpPut("payouts/{id:guid}/reject")]
        public async Task<IActionResult> RejectPayout(Guid id, [FromBody] RejectPayoutRequest request)
        {
            await _walletService.RejectPayoutAsync(id, request.Reason);
            return ApiResponseFactory.Ok(new { }, "Payout rejected");
        }

    }
}
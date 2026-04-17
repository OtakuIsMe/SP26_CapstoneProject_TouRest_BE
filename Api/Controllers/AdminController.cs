using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;
        private readonly IAgencyService _agencyService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService, IAuthService authService, IAgencyService agencyService, IUserService userService, IEmailService emailService)
        {
            _logger = logger;
            _adminService = adminService;
            _authService = authService;
            _agencyService = agencyService;
            _userService = userService;
            _emailService = emailService;
        }

        [HttpPut("agency/{id:guid}/approve")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ApproveAgency(Guid id, [FromBody] RegisterRequestDTO createAccount)
        {

            var userId = User.GetUserId();
            _logger.LogInformation("Admin {AdminId} is approving agency {AgencyId}", userId, id);

            var agency = await _agencyService.GetAgencyById(id);
            if (agency == null)
            {
                return NotFound("Agency not found");
            }


            var agencyCreatorId = agency.CreatedByUserId;
            var agencyCreator = await _userService.GetByIdAsync(agencyCreatorId);
            var email = agencyCreator.Email;

            await _authService.RegisterAsync(createAccount);
            await _adminService.ApproveAgency(id);
            try
            {
                await _emailService.SendAsync(
                    email,
        "Your Agency Has Been Approved — Account Details",
        $@"<h1>Congratulations!</h1>
       <p>Your agency <strong>{agency.Name}</strong> has been approved.</p>
       <h3>Your login credentials:</h3>
       <p>Email: <strong>{createAccount.Email}</strong></p>
       <p>Password: <strong>{createAccount.Password}</strong></p>
       <p>Please log in and change your password immediately.</p>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send approval email to {Email}", email);
            }
            return ApiResponseFactory.Ok(new { }, "Agency approved successfully");
        }

        [HttpPut("agency/{id:guid}/reject")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RejectAgency(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is rejecting agency {AgencyId}", userId, id);

            await _adminService.RejectAgency(id);
            return ApiResponseFactory.Ok(new { }, "Agency rejected successfully");
        }

        [HttpPut("provider/{id:guid}/approve")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ApproveProvider(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is approving provider {ProviderId}", userId, id);

            await _adminService.ApproveProvider(id);
            return ApiResponseFactory.Ok(new { }, "Provider approved successfully");
        }

        [HttpPut("provider/{id:guid}/reject")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RejectProvider(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is rejecting provider {ProviderId}", userId, id);

            await _adminService.RejectProvider(id);
            return ApiResponseFactory.Ok(new { }, "Provider rejected successfully");
        }

        [HttpPost("agency/{id:guid}/create-account")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateAgencyAccount(Guid id, [FromBody] CreateAgencyAccountRequest request)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is creating account for agency {AgencyId}", userId, id);

            await _adminService.CreateAgencyAccount(id, request);
            return ApiResponseFactory.Ok(new { }, "Agency account created successfully");
        }

        [HttpPost("provider/{id:guid}/create-account")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateProviderAccount(Guid id, [FromBody] CreateProviderAccountRequest request)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is creating account for provider {ProviderId}", userId, id);

            await _adminService.CreateProviderAccount(id, request);
            return ApiResponseFactory.Ok(new { }, "Provider account created successfully");
        }

        [HttpPut("user/{id:guid}/ban")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> BanUser(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is banning user {UserId}", userId, id);

            await _adminService.BanUserAsync(id);
            return ApiResponseFactory.Ok(new { }, "User banned successfully");
        }

        [HttpPut("user/{id:guid}/unban")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UnbanUser(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is unbanning user {UserId}", userId, id);

            await _adminService.UnbanUserAsync(id);
            return ApiResponseFactory.Ok(new { }, "User unbanned successfully");
        }

        [HttpGet("pending-agencies")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetPendingAgencies()
        {
            var search = new AgencySearch { Status = AgencyStatus.Pending };
            var result = await _adminService.GetAgencies(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("pending-providers")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetPendingProviders()
        {
            var search = new ProviderSearch { Status = ProviderStatus.Pending };
            var result = await _adminService.GetProviders(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("agency/search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SearchAgencies([FromQuery] AgencySearch search)
        {
            var result = await _adminService.GetAgencies(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("provider/search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SearchProviders([FromQuery] ProviderSearch search)
        {
            var result = await _adminService.GetProviders(search);
            return ApiResponseFactory.Ok(result);
        }

        [HttpGet("users/search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SearchUsers([FromQuery] UserSearch search)
        {
            var result = await _adminService.GetUsers(search);
            return ApiResponseFactory.Ok(result);
        }
    }
}
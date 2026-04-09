using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Interfaces;
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
        public AdminController(ILogger<AdminController> logger, IAdminService adminService)
        {
            _logger = logger;
            _adminService = adminService;
        }
        [HttpPut("agency/{id:guid}/approve")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ApproveAgency(Guid id)
        {
            var userId = User.GetUserRole();
            _logger.LogInformation("Admin {AdminId} is approving agency {AgencyId}", userId, id);
            await _adminService.ApproveAgency(id);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.Interfaces;

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
            await _adminService.ApproveAgency(id);
            return ApiResponseFactory.Ok(new { }, "Agency approved successfully");
        }
         //[HttpPut("agency/{id:guid}/reject")]
         //[Authorize(Roles = "ADMIN")]
         //public async Task<IActionResult> RejectAgency(Guid id, string reason)
         //   {
    }
}

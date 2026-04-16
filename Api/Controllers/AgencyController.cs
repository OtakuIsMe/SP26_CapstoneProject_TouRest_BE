using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/agency")]
    [ApiController]
    public class AgencyController : ControllerBase
    {
        private readonly ILogger<AgencyController> _logger;
        private readonly IAgencyService _agencyService;
        private readonly IAgencyUserService _agencyUserService;
        private readonly IAuthService _authService;
        public AgencyController(ILogger<AgencyController> logger, IAgencyService agencyService,
            IAgencyUserService agencyUserService, IAuthService authService)
        {
            _logger = logger;
            _agencyService = agencyService;
            _agencyUserService = agencyUserService;
            _authService = authService;
        }
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> GetAgencyById(Guid id)
        {
            var agency = await _agencyService.GetAgencyById(id);
            if (agency == null)
                return NotFound();
            return ApiResponseFactory.Ok(agency);

        }
        [HttpGet("user-list")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> GetAgencyUsers(Guid agencyId)
        {
            var users = await _agencyUserService.GetAgencyUsers(agencyId);
            return ApiResponseFactory.Ok(users);
        }
        [HttpGet("me")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> GetMyAgency()
        {
            var user = User.GetUserId();
            var result = await _agencyService.GetMyAgency(user);
            return ApiResponseFactory.Ok(result);
        }
        [HttpPost("{id:guid}/add-user")]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> AddUserToAgency(Guid userId, Guid agencyId, string role)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is adding user {AddedUserId} to agency {AgencyId}", user, userId, agencyId);
            await _agencyUserService.AddUserToAgencyAsync(agencyId, userId, role);
            return ApiResponseFactory.Ok(new { }, "User added to agency");
        }
        [HttpPost("{id:guid}/remove-user")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> RemoveUserFromAgency(Guid id, Guid userId)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is removing user {RemovedUserId} from agency {AgencyId}", user, userId, id);
            await _agencyUserService.RemoveUserFromAgencyAsync(id, userId);
            return ApiResponseFactory.Ok(new { }, "User removed from agency");
        }
        [HttpPost]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CreateAgency(AgencyCreateRequestDTO request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is creating an agency with name {AgencyName}", user, request.Name);
            var result = await _agencyService.AddAgency(user, request);
            return ApiResponseFactory.Created(result, "Agency created. Please wait for Administrator to approve");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAgency(Guid agencyId, [FromBody] AgencyUpdateRequestDTO request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is updating agency {AgencyId}", user, agencyId);
            var result = await _agencyService.UpdateAgency(agencyId, request);
            return ApiResponseFactory.Ok(result, "Agency updated");
        }



    }
}

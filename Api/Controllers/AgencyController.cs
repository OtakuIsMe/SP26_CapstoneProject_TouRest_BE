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
        public AgencyController(ILogger<AgencyController> logger, IAgencyService agencyService, IAgencyUserService agencyUserService)
        {
            _logger = logger;
            _agencyService = agencyService;
            _agencyUserService = agencyUserService;
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
            var users = await _agencyService.GetAgencyUsers(agencyId);
            return ApiResponseFactory.Ok(users);
        }
        [HttpPost("{id:guid}/add-user")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> AddUserToAgency(Guid id, [FromBody] Guid userId)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is adding user {AddedUserId} to agency {AgencyId}", user, userId, id);
            await _agencyUserService.AddUserToAgencyAsync(id, userId);
            return ApiResponseFactory.Ok(new { }, "User added to agency");
        }
        [HttpPost("{id:guid}/remove-user")]
        [Authorize(Roles = "ADMIN, AGENCY")]
        public async Task<IActionResult> RemoveUserFromAgency(Guid id, [FromBody] Guid userId)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is removing user {RemovedUserId} from agency {AgencyId}", user, userId, id);
            await _agencyUserService.RemoveUserFromAgencyAsync(id, userId);
            return ApiResponseFactory.Ok(new { }, "User removed from agency");
        }
        [HttpPost]
        [Authorize(Roles = "AGENCY")]
        public async Task<IActionResult> CreateAgency([FromBody] AgencyCreateRequestDTO request)
        {
            var user = User.GetUserId();
            _logger.LogInformation("User {UserId} is creating an agency with name {AgencyName}", user, request.Name);
            var result = await _agencyService.AddAgency(request);
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

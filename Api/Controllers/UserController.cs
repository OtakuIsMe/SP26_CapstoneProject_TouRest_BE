using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.Common.Constants;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
        [ApiController]
        [Route("api/users")]
        [Authorize]
        public class UserController : ControllerBase
        {
            private readonly IUserService _userService;

            public UserController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpGet("{id:guid}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var user = await _userService.GetByIdAsync(id);

                return ApiResponseFactory.Ok(user);
            }

            [HttpGet("")]
            [Authorize(Roles = RoleCodes.Admin)]
            public async Task<IActionResult> GetUsers()
            {
                var users = await _userService.GetUsers();
                return ApiResponseFactory.Ok(users);
            }
        }
}

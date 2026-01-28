using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
        [ApiController]
        [Route("api/users")]
        public class UserController : ControllerBase
        {
            private readonly IUserService _userService;

            public UserController(IUserService userService)
            {
                _userService = userService;
            }

            /// <summary>
            /// Get user by id
            /// </summary>
            [HttpGet("{id:guid}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var user = await _userService.GetByIdAsync(id);

                return ApiResponseFactory.Ok(user);
            }

            [HttpGet("")]
            public async Task<IActionResult> GetUsers()
            {
                var users = await _userService.GetUsers();
                return ApiResponseFactory.Ok(users);
            }
        }
}

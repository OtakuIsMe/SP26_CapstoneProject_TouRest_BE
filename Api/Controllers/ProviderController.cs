using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Application.Services;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly IAuthService _authService;

        public ProviderController(IProviderService providerService, IAuthService authService)
        {
            _providerService = providerService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _providerService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _providerService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Provider not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> Create([FromBody] CreateProviderRequest request)
        {
            var userId = User.GetUserId();
            var result = await _providerService.CreateAsync(userId, request);
            return ApiResponseFactory.Created(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProviderRequest request)
        {
            var result = await _providerService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Provider not found." });
            }

            return Ok(result);
        }

        [HttpPost("register-request")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> RegisterProviderRequest([FromBody] RegisterProviderAccountRequest request)
        {
            var currentUserId = User.GetUserId();

            await _authService.RegisterProviderAccountAsync(currentUserId, request);

            return ApiResponseFactory.Created(new { }, "Provider request registered successfully");
        }

        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var deleted = await _providerService.DeleteAsync(id);
        //    if (!deleted)
        //    {
        //        return NotFound(new { message = "Provider not found." });
        //    }

        //    return NoContent();
        //}
    }
}

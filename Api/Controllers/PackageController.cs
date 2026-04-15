using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.Package;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _packageService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _packageService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Package not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, PROVIDER")]
        public async Task<IActionResult> Create([FromBody] PackageCreateRequest request)
        {
            var result = await _packageService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN, PROVIDER")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PackageUpdateRequest request)
        {
            var result = await _packageService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Package not found." });
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN, PROVIDER")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _packageService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Package not found." });
            }

            return NoContent();
        }
    }
}
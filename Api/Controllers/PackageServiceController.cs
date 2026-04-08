using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.PackageService;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/packages/{packageId:guid}/services")]
    public class PackageServiceController : ControllerBase
    {
        private readonly IPackageServiceService _packageServiceService;

        public PackageServiceController(IPackageServiceService packageServiceService)
        {
            _packageServiceService = packageServiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPackageId(Guid packageId)
        {
            var result = await _packageServiceService.GetByPackageIdAsync(packageId);
            return Ok(result);
        }

        [HttpGet("{serviceId:guid}")]
        public async Task<IActionResult> GetByIds(Guid packageId, Guid serviceId)
        {
            var result = await _packageServiceService.GetByIdsAsync(packageId, serviceId);
            if (result == null)
            {
                return NotFound(new { message = "Package service not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid packageId, [FromBody] PackageServiceCreateRequest request)
        {
            var result = await _packageServiceService.CreateAsync(packageId, request);
            return CreatedAtAction(nameof(GetByIds), new { packageId = result.PackageId, serviceId = result.ServiceId }, result);
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> Update(Guid packageId, Guid serviceId, [FromBody] PackageServiceUpdateRequest request)
        {
            var result = await _packageServiceService.UpdateAsync(packageId, serviceId, request);
            if (result == null)
            {
                return NotFound(new { message = "Package service not found." });
            }

            return Ok(result);
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> Delete(Guid packageId, Guid serviceId)
        {
            var deleted = await _packageServiceService.DeleteAsync(packageId, serviceId);
            if (!deleted)
            {
                return NotFound(new { message = "Package service not found." });
            }

            return NoContent();
        }
    }
}

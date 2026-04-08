using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.Voucher;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _voucherService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _voucherService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Voucher not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherCreateRequest request)
        {
            var result = await _voucherService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VoucherUpdateRequest request)
        {
            var result = await _voucherService.UpdateAsync(id, request);
            if (result == null)
            {
                return NotFound(new { message = "Voucher not found." });
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _voucherService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Voucher not found." });
            }

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Application.DTOs.Report;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("{Guid:id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var report = await _reportService.GetReport(id);
            if (report == null)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(report);
        }
        [HttpGet("/search")]
        public async Task<IActionResult> GetReports([FromQuery] ReportSearch search)
        {
            var reports = await _reportService.GetReports(search);
            return ApiResponseFactory.Ok(reports);
        }
        [HttpPut("{Guid:id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] ReportUpdateRequest update)
        {
            var report = await _reportService.UpdateReport(id, update);
            return ApiResponseFactory.Ok(report);
        }
        [HttpPost]
        public async Task<IActionResult> AddReport([FromBody] ReportCreateRequest create)
        {
            var report = await _reportService.AddReport(create);
            return ApiResponseFactory.Ok(report);
        }
        [HttpDelete("{Guid:id}")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var result = await _reportService.DeleteReport(id);
            if (!result)
            {
                return NotFound();
            }
            return ApiResponseFactory.Ok(result);
        }
    }
    }

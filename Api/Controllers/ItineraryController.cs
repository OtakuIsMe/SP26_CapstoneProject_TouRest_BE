using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Api.Common;
using TouRest.Api.Extensions;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/itineraries")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        private readonly ILogger<ItineraryController> _logger;
        private readonly IItineraryService _itineraryService;
        public ItineraryController(IItineraryService itineraryService, ILogger<ItineraryController> logger)
        {
            _itineraryService = itineraryService;
            _logger = logger;
        }
        // API endpoints for Itinerary
        [HttpGet]
        public async Task<IActionResult> GetItineraries([FromQuery] ItinerarySearch search)
        {
            var itineraries = await _itineraryService.GetItineraries(search);
            return ApiResponseFactory.Ok(itineraries);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetItineraryById(Guid id)
        {
            var itinerary = await _itineraryService.GetItineraryById(id);
            if (itinerary == null)
                return NotFound();
            return ApiResponseFactory.Ok(itinerary);
        }
        [HttpPost]
        [Authorize(Roles = ("AGENCY"))]
        public async Task<IActionResult> AddItinerary([FromBody] ItineraryCreateRequest create)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Adding itinerary for agency {AgencyId}", agencyId);
            var result = await _itineraryService.AddItinerary(agencyId, create);
            return ApiResponseFactory.Created<ItineraryDTO>(result, "Itinerary created");
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItinerary(Guid id, [FromBody] ItineraryUpdateRequest update)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Updating itinerary {ItineraryId} for agency {AgencyId}", id, agencyId);
            var result = await _itineraryService.UpdateItinerary(id, update);
            return ApiResponseFactory.Ok(result,"itinerary updated");
        }
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest status)
        {
            try
            {
                var agencyId = User.GetUserId();
                _logger.LogInformation("Updating itinerary {ItineraryId} status for agency {AgencyId}", id, agencyId);
                var result = await _itineraryService.UpdateItineraryStatus(id, status);
                return ApiResponseFactory.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
            [HttpDelete("{id:guid}")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> DeleteItinerary(Guid id)
        {
            var agencyId = User.GetUserId();
            _logger.LogInformation("Deleting itinerary {ItineraryId} for agency {AgencyId}", id, agencyId);
            try
            {
                await _itineraryService.DeleteItinerary(id);
                return ApiResponseFactory.NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
    }

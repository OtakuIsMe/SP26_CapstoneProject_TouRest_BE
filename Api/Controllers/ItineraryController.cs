using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        private readonly IItineraryService _itineraryService;
        private readonly IItineraryActivityService _itineraryActivityService;
        private readonly IItineraryStopService _itineraryStopService;
        public ItineraryController(IItineraryService itineraryService, IItineraryActivityService itineraryActivityService, IItineraryStopService itineraryStopService)
        {
            _itineraryService = itineraryService;
            _itineraryActivityService = itineraryActivityService;
            _itineraryStopService = itineraryStopService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetItineraries([FromQuery] ItinerarySearch search)
        {
            var itineraries = await _itineraryService.GetItineraries(search);
            return Ok(itineraries);
        }
        [HttpPost]
        [Authorize(Roles = ("AGENCY"))]
        public async Task<IActionResult> AddItinerary([FromBody] ItineraryCreateRequest create)
        {
            var result = await _itineraryService.AddItinerary(create);
            return Ok(result);
        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItinerary(Guid id, [FromBody] ItineraryUpdateRequest update)
        {
            var result = await _itineraryService.UpdateItinerary(id, update);
            return Ok(result);
        }
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = ("ADMIN, AGENCY"))]
        public async Task<IActionResult> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest status)
        {
            try
            {
                var result = await _itineraryService.UpdateItineraryStatus(id, status);
                return NoContent();
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
            var result = await _itineraryService.DeleteItinerary(id);
            if (result)
                return Ok();
            else
                return NotFound();
        }

    }
    }

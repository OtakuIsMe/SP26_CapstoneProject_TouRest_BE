using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
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
        public async Task<IActionResult> GetItineraries([FromBody] ItinerarySearch search)
        {
            var itineraries = await _itineraryService.GetItineraries(search);
            return Ok(itineraries);
        }
        [HttpPost]
        public async Task<IActionResult> AddItinerary([FromBody] ItineraryCreateRequest create)
        {
            var itinerary = await _itineraryService.AddItinerary(create);
            return Ok(itinerary);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateItinerary(Guid id, [FromBody] ItineraryUpdateRequest update)
        {
            var itinerary = await _itineraryService.UpdateItinerary(id, update);
            return Ok(itinerary);
        }
        [HttpDelete("{id:guid}")]
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

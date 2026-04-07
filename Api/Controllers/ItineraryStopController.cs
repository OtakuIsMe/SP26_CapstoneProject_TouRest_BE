using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TouRest.Api.Common;
using TouRest.Application.Common.Helpers;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Application.Interfaces;

namespace TouRest.Api.Controllers
{
    [Route("api/stop")]
    [ApiController]
    public class ItineraryStopController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IItineraryStopService _itineraryStopService;
        private readonly IItineraryService _itineraryService;
        public ItineraryStopController(IHttpContextAccessor httpContextAccessor, IItineraryStopService itineraryStopService, IItineraryService itineraryService)
        {
            _httpContextAccessor = httpContextAccessor;
            _itineraryStopService = itineraryStopService;
            _itineraryService = itineraryService;
        }
        [HttpGet("itinerary/{itineraryId:guid}")]
        public async Task<IActionResult> GetItineraryStopsByItineraryId(Guid itineraryId)
        {
            var stops = await _itineraryStopService.GetItineraryStopsByItineraryId(itineraryId);
            return ApiResponseFactory.Ok(stops);
        }
        [HttpPost]
        public async Task<IActionResult> AddItineraryStop([FromBody] ItineraryStopCreateRequest create, Guid itineraryId)
        {
            var itinerary = await _itineraryService.GetItineraryById(itineraryId);
            if (itinerary == null)
            {
                return ApiResponseFactory.NoContent("Itinerary not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || itinerary.AgencyId.ToString() != userId)
            {
                return ApiResponseFactory.NoContent("Unauthorized");
            }

            var result = await _itineraryStopService.AddItineraryStop(create, itineraryId);
            return ApiResponseFactory.Created(result);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteItineraryStop(Guid id)
        {
            var stop = await _itineraryStopService.GetItineraryStopById(id);
            if (stop == null)
            {
                return ApiResponseFactory.NoContent("Itinerary stop not found");
            }
            var itinerary = await _itineraryService.GetItineraryById(stop.Itinerary.Id);
            if (itinerary == null)
            {
                return ApiResponseFactory.NoContent("Itinerary not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || itinerary.AgencyId.ToString() != userId)
            {
                return ApiResponseFactory.NoContent("Unauthorized");
            }
            var result = await _itineraryStopService.DeleteItineraryStop(id);
            if (!result)
            {
                return ApiResponseFactory.NoContent("Failed to delete itinerary stop");
            }
            return ApiResponseFactory.Ok("Itinerary stop deleted successfully");
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateItineraryStop(Guid id, [FromBody] ItineraryStopUpdateRequest update)
        {
            var stop = await _itineraryStopService.GetItineraryStopById(id);
            if (stop == null)
            {
                return ApiResponseFactory.NoContent("Itinerary stop not found");
            }
            var itinerary = await _itineraryService.GetItineraryById(stop.Itinerary.Id);
            if (itinerary == null)
            {
                return ApiResponseFactory.NoContent("Itinerary not found");
            }
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || itinerary.AgencyId.ToString() != userId)
            {
                return ApiResponseFactory.NoContent("Unauthorized");
            }
            var result = await _itineraryStopService.UpdateItineraryStop(id, update);
            return ApiResponseFactory.Ok(result, "Itinerary stop updated successfully");
        }
    }
}

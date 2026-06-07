using TouRest.Application.DTOs.Tracking;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryTrackingService : IItineraryTrackingService
    {
        private readonly IItineraryTrackingRepository _repo;
        private readonly IItineraryScheduleRepository _scheduleRepo;
        private readonly IItineraryScheduleService _scheduleService;

        public ItineraryTrackingService(
            IItineraryTrackingRepository repo,
            IItineraryScheduleRepository scheduleRepo,
            IItineraryScheduleService scheduleService)
        {
            _repo = repo;
            _scheduleRepo = scheduleRepo;
            _scheduleService = scheduleService;
        }

        public async Task<List<ItineraryTrackingDTO>> GetByScheduleIdAsync(Guid scheduleId)
        {
            var records = await _repo.GetByScheduleIdAsync(scheduleId);
            return records.Select(MapToDTO).ToList();
        }

        public async Task<ItineraryTrackingDTO> TrackAsync(TrackRequest request)
        {
            var existing = await _repo.FindAsync(request.ItineraryScheduleId, request.TrackingId, request.Type);
            if (existing != null)
                return MapToDTO(existing);

            var entity = new ItineraryTracking
            {
                Id                  = Guid.NewGuid(),
                ItineraryScheduleId = request.ItineraryScheduleId,
                TrackingId          = request.TrackingId,
                Type                = request.Type
            };

            var created = await _repo.CreateAsync(entity);

            await TryMarkScheduleOngoingAsync(request.ItineraryScheduleId);

            if (request.Type == ItineraryTrackingType.Activity)
                await TryAutoCompleteScheduleAsync(request.ItineraryScheduleId);

            return MapToDTO(created);
        }

        private async Task TryMarkScheduleOngoingAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null) return;

            if (schedule.Status is ItineraryScheduleStatus.Ongoing
                or ItineraryScheduleStatus.Completed
                or ItineraryScheduleStatus.Cancelled)
                return;

            await _scheduleService.UpdateStatusAsync(scheduleId, ItineraryScheduleStatus.Ongoing);
        }

        private async Task TryAutoCompleteScheduleAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepo.GetWithStopsAndActivitiesAsync(scheduleId);
            if (schedule == null) return;

            if (schedule.Status is ItineraryScheduleStatus.Completed or ItineraryScheduleStatus.Cancelled)
                return;

            var allActivityIds = schedule.Itinerary.Stops
                .SelectMany(s => s.Activities)
                .Select(a => a.Id)
                .ToHashSet();

            if (allActivityIds.Count == 0) return;

            var tracked = await _repo.GetByScheduleIdAsync(scheduleId);
            var trackedActivityIds = tracked
                .Where(t => t.Type == ItineraryTrackingType.Activity)
                .Select(t => t.TrackingId)
                .ToHashSet();

            if (!allActivityIds.All(id => trackedActivityIds.Contains(id))) return;

            await _scheduleService.UpdateStatusAsync(scheduleId, ItineraryScheduleStatus.Completed);
        }

        public async Task<bool> UntrackAsync(Guid scheduleId, Guid trackingId, ItineraryTrackingType type)
        {
            var existing = await _repo.FindAsync(scheduleId, trackingId, type);
            if (existing == null) return false;
            return await _repo.DeleteAsync(existing.Id);
        }

        private static ItineraryTrackingDTO MapToDTO(ItineraryTracking e) => new()
        {
            Id = e.Id,
            ItineraryScheduleId = e.ItineraryScheduleId,
            TrackingId = e.TrackingId,
            Type = (int)e.Type,
            CreatedAt = e.CreatedAt
        };
    }
}

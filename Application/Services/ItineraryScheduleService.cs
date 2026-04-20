using AutoMapper;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryScheduleService : IItineraryScheduleService
    {
        private readonly IItineraryScheduleRepository _repo;
        private readonly IMapper _mapper;

        public ItineraryScheduleService(IItineraryScheduleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId)
        {
            var list = await _repo.GetByItineraryIdAsync(itineraryId);
            return _mapper.Map<List<ItineraryScheduleDTO>>(list);
        }

        public async Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request)
        {
            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.");

            var schedule = new ItinerarySchedule
            {
                Id = Guid.NewGuid(),
                ItineraryId = itineraryId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                CreatedAt = DateTime.UtcNow,
            };
            var saved = await _repo.CreateAsync(schedule);
            return _mapper.Map<ItineraryScheduleDTO>(saved);
        }

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            return await _repo.DeleteAsync(scheduleId);
        }
    }
}

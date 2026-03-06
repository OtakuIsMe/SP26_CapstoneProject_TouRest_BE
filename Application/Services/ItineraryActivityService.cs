using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.ItineraryActivity;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryActivityService : IItineraryActivityService
    {
        private readonly IItineraryActivityRepository _itineraryActivityRepository;
        private readonly IMapper _mapper;
        public ItineraryActivityService(IItineraryActivityRepository itineraryActivityRepository, IMapper mapper)
        {
            _itineraryActivityRepository = itineraryActivityRepository;
            _mapper = mapper;
        }

        public async Task<List<ItineraryActivityDTO>> GetActivitiesByItineraryStopId(Guid itineraryStopId)
        {
            var list = await _itineraryActivityRepository.GetActivitiesByItineraryStopId(itineraryStopId);
            return _mapper.Map<List<ItineraryActivityDTO>>(list);
        }
        public async Task<ItineraryActivityDTO> GetItineraryActivity(Guid id)
        {
            var itineraryActivity = await _itineraryActivityRepository.GetItineraryActivity(id);
            return _mapper.Map<ItineraryActivityDTO>(itineraryActivity);
        }
        public async Task<ItineraryActivityDTO> AddItineraryActivity(ItineraryActivityCreateRequest create)
        {
            var itineraryActivity = _mapper.Map<ItineraryActivity>(create);
            var result = await _itineraryActivityRepository.CreateAsync(itineraryActivity);
            return _mapper.Map<ItineraryActivityDTO>(result);
        }
        public async Task<bool> DeleteItineraryActivity(Guid id)
        {
            var result = await _itineraryActivityRepository.DeleteAsync(id);
            return result;
        }
        public async Task<ItineraryActivityDTO> UpdateItineraryActivity(Guid id, ItineraryActivityUpdateRequest update)
        {
            var itineraryActivity = _mapper.Map<ItineraryActivity>(update);
            itineraryActivity.Id = id;
            var result = await _itineraryActivityRepository.UpdateAsync(itineraryActivity);
            return _mapper.Map<ItineraryActivityDTO>(result);
        }
    }
}

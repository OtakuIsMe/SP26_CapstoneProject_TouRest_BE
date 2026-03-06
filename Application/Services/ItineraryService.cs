using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryService : IItineraryService
    {
        private readonly IItineraryRepository _itineraryRepository;
        private readonly IMapper _mapper;
        public ItineraryService(IItineraryRepository itineraryRepository, IMapper mapper)
        {
            _itineraryRepository = itineraryRepository;
            _mapper = mapper;
        }

        public async Task<ItineraryDTO> AddItinerary(ItineraryCreateRequest create)
        {
            var itinerary = _mapper.Map<Itinerary>(create);
            var result = await _itineraryRepository.CreateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }

        public async Task<bool> DeleteItinerary(Guid id)
        {
            var result = await _itineraryRepository.DeleteAsync(id);
            return result;
        }

        public async Task<List<ItineraryDTO>> GetItineraries(ItinerarySearch search)
        {
            var list = await _itineraryRepository.GetItineraries(search);
            if(list == null)
            {
                return new List<ItineraryDTO>();
            }
            return _mapper.Map<List<ItineraryDTO>>(list);
        }

        public async Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateRequest update)
        {
            var itinerary = _mapper.Map<Itinerary>(update);
            itinerary.Id = id;
            var result = await _itineraryRepository.UpdateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }
    }
}

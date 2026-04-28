using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Helpers;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
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

        public async Task<ItineraryDTO> AddItinerary(Guid agencyId, ItineraryCreateRequest create)
        {
            var itinerary = _mapper.Map<Itinerary>(create);
            itinerary.AgencyId = agencyId;
            itinerary.Status = ItineraryStatus.Draft; 
            itinerary.SpotLeft = create.MaxCapacity;  
            itinerary.CreatedAt = DateTime.UtcNow;
            itinerary.UpdatedAt = DateTime.UtcNow;


            var result = await _itineraryRepository.CreateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }

        public async Task DeleteItinerary(Guid id)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if(itinerary == null)
            {
                throw new KeyNotFoundException("Itinerary not found");
            }
            itinerary.Status = ItineraryStatus.Inactive;
            await _itineraryRepository.UpdateAsync(itinerary);
        }

        public async Task<List<ItineraryDTO>> GetItineraries(ItinerarySearch search)
        {
            var list = await _itineraryRepository.GetItineraries(search);
            return _mapper.Map<List<ItineraryDTO>>(list);
        }

        public async Task<ItineraryDTO> UpdateItinerary(Guid id, ItineraryUpdateRequest update)
        {
            var existing = await _itineraryRepository.GetByIdAsync(id);
            if(existing == null)
                throw new KeyNotFoundException("Itinerary not found");
            if (!ItineraryStatusTransitions.CanTransition(existing.Status, update.Status))
                throw new InvalidOperationException(
                    $"Invalid transition from {existing.Status} to {update.Status}");
            existing.UpdatedAt = DateTime.UtcNow;
            _mapper.Map(update, existing);
            var result = await _itineraryRepository.UpdateAsync(existing);
            return _mapper.Map<ItineraryDTO>(result);
        }
        public async Task<ItineraryDTO?> GetItineraryById(Guid id)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if (itinerary == null)
                return null;
            return _mapper.Map<ItineraryDTO>(itinerary);
        }
        public async Task<ItineraryDTO?> UpdateItineraryStatus(Guid id, ItineraryUpdateStatusRequest request)
        {
            var itinerary = await _itineraryRepository.GetByIdAsync(id);
            if (itinerary == null)
                throw new KeyNotFoundException("Itinerary not found");
            if (!ItineraryStatusTransitions.CanTransition(itinerary.Status, request.Status))
                throw new InvalidOperationException(
                    $"Invalid transition from {itinerary.Status} to {request.Status}");
            itinerary.Status = request.Status;
            itinerary.UpdatedAt = DateTime.UtcNow;
            var result = await _itineraryRepository.UpdateAsync(itinerary);
            return _mapper.Map<ItineraryDTO>(result);
        }
    }
}

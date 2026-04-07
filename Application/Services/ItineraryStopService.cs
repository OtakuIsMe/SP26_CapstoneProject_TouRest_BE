using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Helpers;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryStopService : IItineraryStopService
    {
        private readonly IItineraryStopRepository _itineraryStopRepository;
        private readonly IItineraryRepository _itineraryRepository;
        private readonly IRouteOptimizerService _routeOptimizerService;
        private readonly IMapper _mapper;
        public ItineraryStopService(IItineraryStopRepository itineraryStopRepository, IItineraryRepository itineraryRepository, 
            IRouteOptimizerService routeOptimizerService,IMapper mapper)
        {
            _itineraryStopRepository = itineraryStopRepository;
            _itineraryRepository = itineraryRepository;
            _routeOptimizerService = routeOptimizerService;
            _mapper = mapper;
        }

        public async Task<ItineraryStopDTO> AddItineraryStop(ItineraryStopCreateRequest create, Guid itineraryId)
        {
            var itineraryStop = _mapper.Map<ItineraryStop>(create);
            var itineraryStopList = await _itineraryStopRepository.GetByItineraryIdAsync(itineraryId);
            itineraryStop.Id = Guid.NewGuid();
            itineraryStop.StopOrder = create.StopOrder ?? 0; // Default to 0 if not provided
            itineraryStop.ItineraryId = itineraryId;
            await _itineraryStopRepository.CreateAsync(itineraryStop);
            // Reorder stops if StopOrder is not provided or if there are conflicts
            await _routeOptimizerService.OptimizeStopsAsync(itineraryId);
            return _mapper.Map<ItineraryStopDTO>(itineraryStop);

        }
        public async Task<List<ItineraryStopDTO>> GetItineraryStopsByItineraryId(Guid itineraryId)
        {
            var list = await _itineraryStopRepository.GetByItineraryIdAsync(itineraryId);
            return _mapper.Map<List<ItineraryStopDTO>>(list);
        }
        public async Task<ItineraryStopDTO?> GetItineraryStopById(Guid id)
        {
            var itineraryStop = await _itineraryStopRepository.GetItineraryStop(id);
            return _mapper.Map<ItineraryStopDTO>(itineraryStop);
        }
        public async Task<ItineraryStopDTO> UpdateItineraryStop(Guid id, ItineraryStopUpdateRequest update)
        {
            var itineraryStop = _mapper.Map<ItineraryStop>(update);
            itineraryStop.Id = id;
            var result = await _itineraryStopRepository.UpdateAsync(itineraryStop);
            await _routeOptimizerService.OptimizeStopsAsync(result.ItineraryId);
            return _mapper.Map<ItineraryStopDTO>(result);
        }
        public async Task<bool> DeleteItineraryStop(Guid id)
        {
            var stop = await _itineraryStopRepository.GetItineraryStop(id);
            if(stop == null)
            {
                return false;
            }
            var result = await _itineraryStopRepository.DeleteAsync(id);

            if (result)
            {
                await _routeOptimizerService.OptimizeStopsAsync(stop.ItineraryId);
            }

            return result;

        }
        /*        public async Task<List<ItineraryStopDTO>> GetAllItineraryStops()
                {
                    var list = await _itineraryStopRepository.GetAllAsync();
                    return _mapper.Map<List<ItineraryStopDTO>>(list);
                }*/
    }
}

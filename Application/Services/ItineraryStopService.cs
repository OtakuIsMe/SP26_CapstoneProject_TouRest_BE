using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryStopService : IItineraryStopService
    {
        private readonly IItineraryStopRepository _itineraryStopRepository;
        private readonly IMapper _mapper;
        public ItineraryStopService(IItineraryStopRepository itineraryStopRepository, IMapper mapper)
        {
            _itineraryStopRepository = itineraryStopRepository;
            _mapper = mapper;
        }

        public async Task<ItineraryStopDTO> AddItineraryStop(ItineraryStopCreateDTO create)
        {
            var itineraryStop = _mapper.Map<ItineraryStop>(create);
            await _itineraryStopRepository.CreateAsync(itineraryStop);
            return _mapper.Map<ItineraryStopDTO>(itineraryStop);

        }
        public async Task<List<ItineraryStopDTO>> GetItineraryStopsByItineraryId(Guid itineraryId)
        {
            var list = await _itineraryStopRepository.GetByItineraryIdAsync(itineraryId);
            return _mapper.Map<List<ItineraryStopDTO>>(list);
        }
        public async Task<ItineraryStopDTO> GetItineraryStop(Guid id)
        {
            var itineraryStop = await _itineraryStopRepository.GetItineraryStop(id);
            return _mapper.Map<ItineraryStopDTO>(itineraryStop);
        }
        public async Task<ItineraryStopDTO> UpdateItineraryStop(Guid id, ItineraryStopUpdateDTO update)
        {
            var itineraryStop = _mapper.Map<ItineraryStop>(update);
            itineraryStop.Id = id;
            var result = await _itineraryStopRepository.UpdateAsync(itineraryStop);
            return _mapper.Map<ItineraryStopDTO>(result);
        }
        public async Task<bool> DeleteItineraryStop(Guid id)
        {
            var result = await _itineraryStopRepository.DeleteAsync(id);
            return result;
        }
/*        public async Task<List<ItineraryStopDTO>> GetAllItineraryStops()
        {
            var list = await _itineraryStopRepository.GetAllAsync();
            return _mapper.Map<List<ItineraryStopDTO>>(list);
        }*/
    }
}

using AutoMapper;
using TouRest.Application.DTOs.Provider;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class ProviderProfile : Profile
    {
        public ProviderProfile()
        {
            CreateMap<Provider, ProviderDTO>();
        }
    }
}

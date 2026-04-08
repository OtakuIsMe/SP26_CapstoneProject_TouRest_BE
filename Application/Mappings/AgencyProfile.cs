using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class AgencyProfile : Profile
    {
        public AgencyProfile()
        {

            CreateMap<Agency, AgencyDTO>()
                .ForMember(dest =>dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<AgencyCreateRequestDTO, Agency>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<AgencyUpdateRequestDTO, Agency>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


        }
    }
}

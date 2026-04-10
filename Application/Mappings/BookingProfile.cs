using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Booking;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Application.Mappings
{
    public class BookingProfile : Profile
    {
        //Mapping configurations for Booking entity and related DTOs
         public BookingProfile()
        {
            CreateMap<Booking, BookingSummaryDTO>();
            CreateMap<BookingCreateRequest, Booking>()
            .ForMember(dest => dest.Id, opt => Guid.NewGuid())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(Src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Pending));
            CreateMap<BookingUpdateRequest, Booking>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}

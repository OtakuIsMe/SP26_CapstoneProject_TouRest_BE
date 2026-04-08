using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Booking;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class BookingProfile : Profile
    {
        //Mapping configurations for Booking entity and related DTOs
         public BookingProfile()
        {
            CreateMap<Booking, BookingSummaryDTO>();
            CreateMap<BookingCreateRequest, Booking>();
            CreateMap<BookingUpdateRequest, Booking>();
        }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.DTOs.ItineraryActivity;
using TouRest.Application.DTOs.ItineraryStop;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Mappings
{
    public class ItineraryProfile : Profile
    {
        public ItineraryProfile()
        {
            // Map Itinerary entity to ItineraryDTO for responses
            CreateMap<Itinerary,ItineraryDTO>();
            //Map ItineraryCreateDTO to Itinerary entity for creating new itineraries
            CreateMap<ItineraryCreateDTO, Itinerary>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.ItineraryStatus.Active));
            //Map ItineraryUpdateDTO to Itinerary entity for updating existing itineraries
            CreateMap<ItineraryUpdateDTO, Itinerary>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id should not be updated
                .ForMember(dest => dest.Status, opt => opt.Ignore()); // Status should not be updated here
            //Map ItineraryActivity entity to ItineraryActivityDTO for responses
            CreateMap<ItineraryActivity, ItineraryActivityDTO>();
            //Map ItineraryActivityCreateDTO to ItineraryActivity entity for creating new itinerary activities
            CreateMap<ItineraryActivityCreateDTO, ItineraryActivity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            //Map ItineraryActivityUpdateDTO to ItineraryActivity entity for updating existing itinerary activities
            CreateMap<ItineraryActivityUpdateDTO, ItineraryActivity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id should not be updated
            //Map ItineraryStop entity to ItineraryStopDTO for responses
            CreateMap<ItineraryStop, ItineraryStopDTO>();
            //Map ItineraryStopCreateDTO to ItineraryStop entity for creating new itinerary stops
            CreateMap<ItineraryStopCreateDTO, ItineraryStop>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
            //Map ItineraryStopUpdateDTO to ItineraryStop entity for updating existing itinerary stops
            CreateMap<ItineraryStopUpdateDTO, ItineraryStop>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id should not be updated
            //Map ItineraryActvity to ItineraryActivitySummaryDTO for summary responses
            CreateMap<ItineraryActivity, ItineraryActivitySummaryDTO>();
            //Map Itinerary to ItinerarySummaryDTO for summary responses
            CreateMap<Itinerary, ItinerarySummaryDTO>();
            //Map ItineraryStop to ItineraryStopSummaryDTO for summary responses
            CreateMap<ItineraryStop, ItineraryStopSummaryDTO>();

        }
    }
}

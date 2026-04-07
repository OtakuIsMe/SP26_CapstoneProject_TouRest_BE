using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Feedback;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Feedback, FeedbackDTO>();
            // For creating new feedback, we don't want to include Id, CreatedAt, UpdatedAt, and BookingItinerary navigation property
            CreateMap<FeedbackCreateRequest, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.BookingItinerary, opt => opt.Ignore());
            // For updating feedback, we want to ignore Id, CreatedAt, and BookingItinerary navigation property
            CreateMap<FeedbackUpdateRequest, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.BookingItinerary, opt => opt.Ignore());
            // For feedback summary, we only want to include Id, Rating, Title, and CreatedAt
            CreateMap<Feedback, FeedbackSummaryDTO>();
        }
    }
}

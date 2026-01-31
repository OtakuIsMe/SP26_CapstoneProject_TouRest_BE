using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.User;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Map User entity to UserDTO for responses
            CreateMap<User, UserDTO>();

            // Map RegisterRequestDTO to User entity
            // Note: PasswordHash will be set manually in the service layer using IPasswordHasher
            CreateMap<RegisterRequestDTO, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.UserStatus.Active))
                .ForMember(dest => dest.RoleId, opt => opt.Ignore()); // Will be set in service
        }
    }
}

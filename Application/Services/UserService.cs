using AutoMapper;
using TouRest.Application.DTOs.User;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Application.Common.Exceptions;
using TouRest.Domain.Enums;

namespace TouRest.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IImageRepository imageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with id {id} not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return users;
        }

        public async Task<UserDTO> UpdateProfileAsync(Guid userId, UpdateProfileDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with id {userId} not found");

            if (dto.Username != null) user.Username = dto.Username;
            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.DateOfBirth.HasValue) user.DateOfBirth = dto.DateOfBirth;
            if (dto.AddressDetail != null) user.AddressDetail = dto.AddressDetail;
            if (dto.CityId != null) user.CityId = dto.CityId;
            if (dto.DistrictId != null) user.DistrictId = dto.DistrictId;

            if (dto.ImageUrl != null)
            {
                var image = await _imageRepository.CreateAsync(new Image
                {
                    Url = dto.ImageUrl,
                    Type = ImageType.User
                });
                user.ImageId = image.Id;
            }

            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDTO>(updated);
        }
    }
}

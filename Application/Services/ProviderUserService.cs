using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ProviderUserService : IProviderUserService
    {
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ProviderUserService(
            IProviderUserRepository providerUserRepository,
            IProviderRepository providerRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher)
        {
            _providerUserRepository = providerUserRepository;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ProviderStaffDTO> CreateStaffAccountAsync(Guid providerId, CreateProviderStaffRequest request)
        {
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var providerRole = await _roleRepository.GetByCodeAsync(RoleCodes.Provider);
            if (providerRole == null)
                throw new InvalidOperationException("PROVIDER role not found.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = providerRole.Id,
                Status = UserStatus.Active,
            };
            await _userRepository.CreateAsync(user);
            await _providerUserRepository.AddUserIntoProvider(providerId, user.Id, ProviderUserRole.Staff);

            return new ProviderStaffDTO
            {
                ProviderId = providerId,
                UserId = user.Id,
                UserFullName = user.FullName ?? user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = ProviderUserRole.Staff.ToString(),
            };
        }

        public async Task<List<ProviderStaffDTO>> GetStaffAsync(Guid providerId)
        {
            if (providerId == Guid.Empty)
                throw new ArgumentException("ProviderId cannot be empty.", nameof(providerId));

            var staff = await _providerUserRepository.GetStaffByProviderIdAsync(providerId);
            return staff.Select(pu => new ProviderStaffDTO
            {
                ProviderId = pu.ProviderId,
                UserId = pu.UserId,
                UserFullName = pu.User.FullName ?? pu.User.Username,
                Email = pu.User.Email,
                Phone = pu.User.Phone,
                Avatar = pu.User.UserAvatar,
                Role = pu.Role.ToString(),
            }).ToList();
        }

        public async Task RemoveStaffAsync(Guid providerId, Guid userId)
        {
            if (providerId == Guid.Empty)
                throw new ArgumentException("ProviderId cannot be empty.", nameof(providerId));
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            await _providerUserRepository.RemoveUserFromProviderAsync(providerId, userId);
        }

        public async Task AddUserToProviderAsync(Guid providerId, Guid userId, ProviderUserRole role)
        {
            if (await _providerRepository.GetByIdAsync(providerId) == null)
                throw new KeyNotFoundException("Provider not found.");
            if (await _userRepository.GetByIdAsync(userId) == null)
                throw new KeyNotFoundException("User not found.");

            await _providerUserRepository.AddUserIntoProvider(providerId, userId, role);
        }

        public async Task<ProviderStaffDTO?> GetProviderUserByUserId(Guid userId)
        {
            var pu = await _providerUserRepository.GetByUserIdAsync(userId);
            if (pu == null) return null;
            return new ProviderStaffDTO
            {
                ProviderId = pu.ProviderId,
                UserId = pu.UserId,
                UserFullName = pu.User.FullName ?? pu.User.Username,
                Email = pu.User.Email,
                Phone = pu.User.Phone,
                Avatar = pu.User.UserAvatar,
                Role = pu.Role.ToString(),
            };
        }
    }
}

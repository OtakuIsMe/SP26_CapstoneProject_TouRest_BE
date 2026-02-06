using AutoMapper;
using TouRest.Application.Common.Constants;
using TouRest.Application.Common.Exceptions;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Domain.Enums;

namespace TouRest.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<(AuthResponseDTO auth, string refreshToken)> LoginAsync(LoginRequestDTO request)
        {
            // 1. Validate user credentials
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // 2. Check if user exists and password is correct
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            // 3. Check if user account is active
            if (user.Status != UserStatus.Active)
                throw new UnauthorizedAccessException("User account is not active");

            // 4. Generate JWT tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays),
            });

            return (new AuthResponseDTO
            {
                AccessToken = accessToken,
                ExpiresIn = AuthConstants.AccessTokenExpiryMinutes * 60
            }, refreshToken);
        }

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var userRole = await _roleRepository.GetByCodeAsync(RoleCodes.Customer);
            if (userRole == null)
                throw new InvalidOperationException("Default user role not found in database");

            var user = _mapper.Map<User>(request);

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.RoleId = userRole.Id;
            await _userRepository.CreateAsync(user);
        }

        public async Task<(AuthResponseDTO auth, string refreshToken)> RefreshTokenAsync(string refreshTokenValue, Guid userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);
            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (refreshToken.UserId != userId)
                throw new UnauthorizedAccessException("Refresh token does not match user");

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            await _refreshTokenRepository.RevokeAsync(refreshTokenValue);

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            await _refreshTokenRepository.CreateAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenExpiryDays)
            });

            return (new AuthResponseDTO
            {
                AccessToken = newAccessToken,
                ExpiresIn = AuthConstants.AccessTokenExpiryMinutes * 60
            }, newRefreshToken);
        }

        public async Task LogoutAsync(string refreshTokenValue, Guid userId)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);
            if (refreshToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (refreshToken.UserId != userId)
                throw new UnauthorizedAccessException("Refresh token does not match user");

            await _refreshTokenRepository.RevokeAsync(refreshTokenValue);
        }
    }
}
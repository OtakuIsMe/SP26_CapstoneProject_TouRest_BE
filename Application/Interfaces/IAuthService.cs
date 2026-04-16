using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.Provider;

namespace TouRest.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResponseDTO auth, string refreshToken)> LoginAsync(LoginRequestDTO request);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request, string refreshToken);
        Task ChangeRoleAsync(Guid userId, ChangeRoleRequestDTO request, string refreshToken);
        Task RegisterAsync(RegisterRequestDTO request);
        Task<(AuthResponseDTO auth, string refreshToken)> RefreshTokenAsync(string refreshToken, Guid userId);
        Task LogoutAsync(string refreshToken, Guid userId);
        Task<MeDTO> GetMeAsync(Guid userId);
        Task RegisterProviderAccountAsync(Guid createdByUserId, RegisterProviderAccountRequest request);
        Task RegisterAgencyAccountAsync(Guid createdByUserId, RegisterAgencyAccountRequest request);
    }
}

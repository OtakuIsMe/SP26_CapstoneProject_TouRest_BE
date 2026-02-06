using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Auth;

namespace TouRest.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResponseDTO auth, string refreshToken)> LoginAsync(LoginRequestDTO request);
        Task RegisterAsync(RegisterRequestDTO request);
        Task<(AuthResponseDTO auth, string refreshToken)> RefreshTokenAsync(string refreshToken, Guid userId);
        Task LogoutAsync(string refreshToken, Guid userId);
    }
}

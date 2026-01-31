using System;

namespace TouRest.Application.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public required string AccessToken { get; set; }

        public required int ExpiresIn { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Auth
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; } = null!;
    }
}

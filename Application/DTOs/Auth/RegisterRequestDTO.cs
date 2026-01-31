using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace TouRest.Application.DTOs.User
{
    public class UpdateProfileDTO
    {
        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? AddressDetail { get; set; }

        [MaxLength(100)]
        public string? CityId { get; set; }

        [MaxLength(100)]
        public string? DistrictId { get; set; }
    }
}

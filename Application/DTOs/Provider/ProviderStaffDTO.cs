namespace TouRest.Application.DTOs.Provider
{
    public class ProviderStaffDTO
    {
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string Role { get; set; } = null!;
    }
}

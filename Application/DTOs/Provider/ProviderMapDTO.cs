namespace TouRest.Application.DTOs.Provider
{
    public class ProviderMapDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Address { get; set; }
        public string? ContactPhone { get; set; }
    }
}

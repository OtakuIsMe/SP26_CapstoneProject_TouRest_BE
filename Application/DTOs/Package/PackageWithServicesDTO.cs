using TouRest.Application.DTOs.PackageService;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Package
{
    public class PackageWithServicesDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int BasePrice { get; set; }
        public PackageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PackageServiceDTO> Services { get; set; } = [];
    }
}

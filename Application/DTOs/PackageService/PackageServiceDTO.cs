using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.PackageService
{
    public class PackageServiceDTO
    {
        public Guid PackageId { get; set; }
        public Guid ServiceId { get; set; }
        public int SortOrder { get; set; }

        public string ServiceName { get; set; } = null!;
        public string? ServiceDescription { get; set; }
        public int ServicePrice { get; set; }
        public int ServiceDurationMinutes { get; set; }
        public ServiceStatus ServiceStatus { get; set; }
        public int ServiceBasePrice { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouRest.Application.DTOs.PackageService
{
    public class PackageServiceSummaryDTO
    {
        public Guid PackageId { get; set; }
        public Guid ServiceId { get; set; }
        public int SortOrder { get; set; }
        public string ServiceName { get; set; } = null!;
    }
}

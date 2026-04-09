using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Provider
{
    public class ProviderDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public ProviderStatus Status { get; set; }
        public string Description { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
    }
}

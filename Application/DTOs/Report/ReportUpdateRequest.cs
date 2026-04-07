using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Report
{
    public class ReportUpdateRequest
    {
        [MaxLength(1000)]
        public string Description { get; set; } = null!;
        public ReportStatus Status { get; set; }
    }
}

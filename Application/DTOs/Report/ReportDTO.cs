using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.DTOs.Report
{
    public class ReportDTO
    {
        
        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;      
        public Guid ItemId { get; set; }
        public ReportItemType ItemType { get; set; }
        public ReportStatus Status { get; set; }
    }
}

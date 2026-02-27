using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Interfaces
{
    public interface IReportRepository : IBaseRepository<Report>
    {
        Task<List<Report>> GetReports(ReportSearch search);
        Task<Report?> GetReport(Guid id);
    }
    public class ReportSearch
    {
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public ReportItemType? ItemType { get; set; }
        public Guid? ItemId { get; set; }
        public ReportStatus? Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Report;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportDTO>> GetReports(ReportSearch search);
        Task<ReportDTO> GetReport(Guid id);
        Task<ReportDTO> UpdateReport(Guid id, ReportUpdateRequest update);
        Task<ReportDTO> AddReport(ReportCreateRequest create);
        Task<bool> DeleteReport(Guid id);
    }
}

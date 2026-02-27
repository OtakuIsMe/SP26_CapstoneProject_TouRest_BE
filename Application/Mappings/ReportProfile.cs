using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Report;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Report, ReportDTO>();
        }
    }
}

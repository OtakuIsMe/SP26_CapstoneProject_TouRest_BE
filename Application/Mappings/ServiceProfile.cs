using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Service;
using TouRest.Domain.Entities;

namespace TouRest.Application.Mappings
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            //Mapping configurations for Service
            CreateMap<Service, ServiceDTO>();
            //Mapping create and update DTOs to entity
            CreateMap<ServiceCreateRequest, Service>();
            CreateMap<ServiceUpdateRequest, Service>();
        }
    }
}

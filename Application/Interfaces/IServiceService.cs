using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IServiceService
    {
        Task<Service?> GetServiceById(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.User;
using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetUsers();
    }
}

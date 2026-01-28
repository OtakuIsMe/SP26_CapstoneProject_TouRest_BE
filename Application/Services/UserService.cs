using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Common.Exceptions;
using TouRest.Application.DTOs;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException($"User with id {id} not found");

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.Username,
                Email = user.Email
            };
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return users;
        }
    }
}

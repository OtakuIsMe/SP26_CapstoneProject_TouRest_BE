using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.DTOs.User;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IAgencyUserRepository _agencyUserRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderUserRepository _providerUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AdminService(IAdminRepository adminRepository, IAgencyRepository agencyRepository,
            IProviderRepository providerRepository, IMapper mapper,
            IAgencyUserRepository agencyUserRepository, IProviderUserRepository providerUserRepository,
            IUserRepository userRepository)
        {
            _adminRepository = adminRepository;
            _agencyRepository = agencyRepository;
            _providerRepository = providerRepository;
            _mapper = mapper;
            _agencyUserRepository = agencyUserRepository;
            _providerUserRepository = providerUserRepository;
            _userRepository = userRepository;
        }
        private async Task Validate(Guid id, string type)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{type}Id cannot be empty", nameof(id));
            if (type == "agency" && await _agencyRepository.GetByIdAsync(id) == null)
                throw new KeyNotFoundException("Agency not found");
            if (type == "provider" && await _providerRepository.GetByIdAsync(id) == null)
                throw new KeyNotFoundException("Provider not found");
            if (type == "user" && await _userRepository.GetByIdAsync(id) == null)
                throw new KeyNotFoundException("User not found");
        }
        public async Task ApproveAgency(Guid agencyId)
        {
            await Validate(agencyId, "agency");
            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency == null)
            {
                throw new KeyNotFoundException("Agency not found");
            }
            await _adminRepository.ApproveAgency(agencyId);
            await _agencyUserRepository.AddUserToAgencyAsync(agencyId, agency.CreateByUserId, AgencyUserRole.Manager);
        }

        public async Task ApproveProvider(Guid providerId)
        {
            await Validate(providerId, "provider");
            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
            {
                throw new KeyNotFoundException("Provider not found");
            }
            await _adminRepository.ApproveProvider(providerId);
        }

        public async Task BanUserAsync(Guid userId)
        {
            await Validate(userId, "user");
            await _adminRepository.BanUserAsync(userId);
        }

        public async Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status)
        {
            await Validate(userId, "user");
            await _adminRepository.DemoteFromAdminAsync(userId, roleId, status);
        }

        public async Task<List<AgencyDTO>> GetAgencies(AgencySearch search)
        {
            var list = await _adminRepository.GetAgencies(search);
            return _mapper.Map<List<AgencyDTO>>(list);
        }

        public async Task<List<ProviderDTO>> GetProviders(ProviderSearch search)
        {
            var list = await _adminRepository.GetProviders(search);
            return _mapper.Map<List<ProviderDTO>>(list);
        }

        public async Task<List<UserDTO>> GetUsers(UserSearch search)
        {
            var list = await _adminRepository.GetUsers(search);
            return _mapper.Map<List<UserDTO>>(list);
        }

        public Task PromoteToAdminAsync(Guid userId)
        {
            throw new NotSupportedException();
        }

        public async Task RejectAgency(Guid agencyId)
        {
            await Validate(agencyId, "agency");
            await _adminRepository.RejectAgency(agencyId);
        }

        public async Task RejectProvider(Guid providerId)
        {
            await Validate(providerId, "provider");
            await _adminRepository.RejectProvider(providerId);
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            await Validate(userId, "user");
            await _adminRepository.UnbanUserAsync(userId);
        }

    }
}

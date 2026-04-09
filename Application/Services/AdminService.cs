using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMapper _mapper;
        public AdminService(IAdminRepository adminRepository, IMapper mapper)
        {
            _adminRepository = adminRepository;
            _mapper = mapper;
        }
        public async Task ApproveAgency(Guid agencyId)
        {
            await _adminRepository.ApproveAgency(agencyId);
        }

        public async Task ApproveProvider(Guid providerId)
        {
            await _adminRepository.ApproveProvider(providerId);
        }

        public async Task BanUserAsync(Guid userId)
        {
            await _adminRepository.BanUserAsync(userId);
        }

        public async Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status)
        {
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
            throw new NotImplementedException();
        }

        public async Task RejectAgency(Guid agencyId)
        {
            await _adminRepository.RejectAgency(agencyId);
        }

        public async Task RejectProvider(Guid providerId)
        {
            await _adminRepository.RejectProvider(providerId);
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            await _adminRepository.UnbanUserAsync(userId);
        }
        
    }
}

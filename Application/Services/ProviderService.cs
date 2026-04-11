using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;


namespace TouRest.Application.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderUserRepository _providerUserRepository;

        public ProviderService(
            IProviderRepository providerRepository,
            IProviderUserRepository providerUserRepository)
        {
            _providerRepository = providerRepository;
            _providerUserRepository = providerUserRepository;
        }

        public async Task<List<ProviderResponse>> GetAllAsync()
        {
            var providers = await _providerRepository.GetAllAsync();
            return providers.Select(MapToResponse).ToList();
        }

        public async Task<ProviderResponse?> GetByIdAsync(Guid id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            return provider == null ? null : MapToResponse(provider);
        }

        public async Task<ProviderResponse> CreateAsync(Guid currentUserId, CreateProviderRequest request)
        {
            var emailExists = await _providerRepository.ExistsByContactEmailAsync(request.ContactEmail);
            if (emailExists)
            {
                throw new InvalidOperationException("Contact email already exists.");
            }

            var existingProvider = await _providerRepository.GetByCreateByUserIdAsync(currentUserId);
            if (existingProvider != null)
            {
                throw new InvalidOperationException("User has already registered a provider.");
            }

            var provider = new Provider
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = ProviderStatus.Pending,
                CreateByUserId = currentUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _providerRepository.AddAsync(provider);

            var providerUser = new ProviderUser
            {
                Id = Guid.NewGuid(),
                ProviderId = provider.Id,
                UserId = currentUserId,
                Role = ProviderUserRole.Manager,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _providerUserRepository.AddAsync(providerUser);
            await _providerRepository.SaveChangesAsync();

            return MapToResponse(provider);
        }

        public async Task<ProviderResponse?> UpdateAsync(Guid id, UpdateProviderRequest request)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            if (provider == null)
            {
                return null;
            }

            var duplicatedEmail = await _providerRepository.GetByContactEmailAsync(request.ContactEmail);
            if (duplicatedEmail != null && duplicatedEmail.Id != id)
            {
                throw new InvalidOperationException("Contact email already exists.");
            }

            provider.Name = request.Name;
            provider.ContactEmail = request.ContactEmail;
            provider.ContactPhone = request.ContactPhone;
            provider.Status = request.Status;
            provider.UpdatedAt = DateTime.UtcNow;

            _providerRepository.Update(provider);
            await _providerRepository.SaveChangesAsync();

            return MapToResponse(provider);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var provider = await _providerRepository.GetByIdAsync(id);
            if (provider == null)
            {
                return false;
            }

            _providerRepository.Remove(provider);
            await _providerRepository.SaveChangesAsync();

            return true;
        }

        private static ProviderResponse MapToResponse(Provider provider)
        {
            return new ProviderResponse
            {
                Id = provider.Id,
                Name = provider.Name,
                Status = provider.Status,
                ContactEmail = provider.ContactEmail,
                ContactPhone = provider.ContactPhone,
                CreatedAt = provider.CreatedAt,
                UpdatedAt = provider.UpdatedAt
            };
        }
    }
}

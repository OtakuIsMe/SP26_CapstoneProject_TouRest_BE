using AutoMapper;
using TouRest.Application.Common.Constants;
using TouRest.Application.DTOs.Agency;
using TouRest.Application.DTOs.Auth;
using TouRest.Application.DTOs.Feedback;
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
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public AdminService(
            IAdminRepository adminRepository,
            IAgencyRepository agencyRepository,
            IAgencyUserRepository agencyUserRepository,
            IProviderRepository providerRepository,
            IProviderUserRepository providerUserRepository,
            IFeedbackRepository feedbackRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            INotificationRepository notificationRepository,
            IMapper mapper)
        {
            _adminRepository = adminRepository;
            _agencyRepository = agencyRepository;
            _agencyUserRepository = agencyUserRepository;
            _providerRepository = providerRepository;
            _providerUserRepository = providerUserRepository;
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }


        public async Task ApproveAgency(Guid agencyId)
        {

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency == null)
                throw new KeyNotFoundException("Agency not found");

            if (agency.Status != AgencyStatus.Pending)
                throw new InvalidOperationException("Only pending agencies can be approved");

            await _adminRepository.ApproveAgency(agencyId);
        }

        public async Task ApproveProvider(Guid providerId)
        {

            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
                throw new KeyNotFoundException("Provider not found");

            if (provider.Status != ProviderStatus.Pending)
                throw new InvalidOperationException("Only pending providers can be approved");

            await _adminRepository.ApproveProvider(providerId);
        }

        public async Task CreateAgencyAccount(Guid agencyId, CreateAgencyAccountRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency == null)
                throw new KeyNotFoundException("Agency not found");

            if (agency.Status != AgencyStatus.Pending && agency.Status != AgencyStatus.Active)
                throw new InvalidOperationException("Only pending or approved agencies can have accounts created");

            if (agency.Status == AgencyStatus.Pending)
                await _adminRepository.ApproveAgency(agencyId);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            //var existingAgencyUsers = await _agencyUserRepository.GetAgencyUsers(agencyId);
            //if (existingAgencyUsers.Any(u => u.Role == AgencyUserRole.Manager))
            //    throw new InvalidOperationException("Agency already has a manager account");

            var agencyRole = await _roleRepository.GetByCodeAsync(RoleCodes.Agency);
            if (agencyRole == null)
                throw new InvalidOperationException("Agency role not found in database");

            var agencyAccount = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = agencyRole.Id,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = agency.CreateByUserId,
                Title = "Your agency has been approved",
                Message = $"Congratulations! Your agency '{agency.Name}' has been approved. You can now log in with email: {request.Email} and password: {request.Password} to manage your agency.",
                EntityType = NotificationEntityType.Other,
                EntityId = agencyId,
                IsRead = false,
            };

            var agencyRoleAssignment = request.Role == default ? AgencyUserRole.Manager : request.Role;

            await _userRepository.CreateAsync(agencyAccount);
            await _agencyUserRepository.AddUserToAgencyAsync(agencyId, agencyAccount.Id, agencyRoleAssignment);
            await _notificationRepository.CreateAsync(notification);
        }

        public async Task CreateProviderAccount(Guid providerId, CreateProviderAccountRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
                throw new KeyNotFoundException("Provider not found");

            if (provider.Status != ProviderStatus.Pending && provider.Status != ProviderStatus.Active)
                throw new InvalidOperationException("Only pending or approved providers can have accounts created");

            if (provider.Status == ProviderStatus.Pending)
                await _adminRepository.ApproveProvider(providerId);

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var providerRole = await _roleRepository.GetByCodeAsync(RoleCodes.Provider);
            if (providerRole == null)
                throw new InvalidOperationException("Provider role not found in database");

            var providerAccount = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = providerRole.Id,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = provider.CreateByUserId,
                Title = "Your provider has been approved",
                Message = $"Congratulations! Your provider '{provider.Name}' has been approved. You can now log in with email: {request.Email} and password: {request.Password} to manage your provider.",
                EntityType = NotificationEntityType.Other,
                EntityId = providerId,
                IsRead = false,
            };

            var providerRoleAssignment = request.Role == default ? ProviderUserRole.Manager : request.Role;

            await _userRepository.CreateAsync(providerAccount);
            await _providerUserRepository.AddUserIntoProvider(providerId, providerAccount.Id, providerRoleAssignment);
            await _notificationRepository.CreateAsync(notification);
        }

        public async Task BanUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) 
                throw new KeyNotFoundException("User not found");
            if (user.Status == UserStatus.Locked)
                throw new InvalidOperationException("User is already banned");
            await _adminRepository.BanUserAsync(userId);
        }

        public async Task DemoteFromAdminAsync(Guid userId, Guid roleId, UserStatus? status)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            if (user.RoleId != roleId)
                throw new InvalidOperationException("User is not an admin");
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
            var agency = await _agencyRepository.GetByIdAsync(agencyId);
            if (agency == null)
                throw new KeyNotFoundException("Agency not found");
            if (agency.Status != AgencyStatus.Pending)
                throw new InvalidOperationException("Only pending agencies can be rejected");

            await _adminRepository.RejectAgency(agencyId);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = agency.CreateByUserId,
                Title = "Your agency registration has been rejected",
                Message = $"We're sorry, your agency registration '{agency.Name}' has been rejected. Please contact support for more information.",
                EntityType = NotificationEntityType.Other,
                EntityId = agencyId,
                IsRead = false,
            };
            await _notificationRepository.CreateAsync(notification);
        }

        public async Task RejectProvider(Guid providerId)
        {
            var provider = await _providerRepository.GetByIdAsync(providerId);
            if (provider == null)
                throw new KeyNotFoundException("Provider not found");
            if (provider.Status != ProviderStatus.Pending)
                throw new InvalidOperationException("Only pending providers can be rejected");

            await _adminRepository.RejectProvider(providerId);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientUserId = provider.CreateByUserId,
                Title = "Your provider registration has been rejected",
                Message = $"We're sorry, your provider registration '{provider.Name}' has been rejected. Please contact support for more information.",
                EntityType = NotificationEntityType.Other,
                EntityId = providerId,
                IsRead = false,
            };
            await _notificationRepository.CreateAsync(notification);
        }

        public async Task UnbanUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            if (user.Status != UserStatus.Locked)
                throw new InvalidOperationException("User is not banned");
            await _adminRepository.UnbanUserAsync(userId);
        }
        public async Task<List<FeedbackDTO>> GetFeedbacks(FeedbackSearch search)
        {
            var list = await _feedbackRepository.GetFeedbacks(search);
            return _mapper.Map<List<FeedbackDTO>>(list);
        }

        public async Task HideFeedback(Guid feedbackId)
        {
            var feedback = await _feedbackRepository.GetFeedback(feedbackId);
            if (feedback == null)
                throw new KeyNotFoundException("Feedback not found");
            if (feedback.Status == FeedbackStatus.Archived)
                throw new InvalidOperationException("Feedback is already hidden");
            feedback.Status = FeedbackStatus.Archived;
            feedback.UpdatedAt = DateTime.UtcNow;
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteFeedback(Guid feedbackId)
        {
            var feedback = await _feedbackRepository.GetFeedback(feedbackId);
            if (feedback == null)
                throw new KeyNotFoundException("Feedback not found");
            feedback.Status = FeedbackStatus.Remove;
            feedback.UpdatedAt = DateTime.UtcNow;
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public Task Dashboard()
        {
            throw new NotImplementedException();
        }
    }
}
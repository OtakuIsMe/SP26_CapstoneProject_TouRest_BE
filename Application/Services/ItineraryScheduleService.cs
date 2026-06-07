using TouRest.Application.DTOs.Itinerary;
using TouRest.Application.DTOs.Provider;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ItineraryScheduleService : IItineraryScheduleService
    {
        private readonly IItineraryScheduleRepository _repo;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IBookingItineraryRepository _bookingItineraryRepository;
        private readonly INotificationRepository      _notificationRepo;
        private readonly IAgencyUserRepository  _agencyUserRepo;
        private readonly IStopStaffAssignmentRepository _assignmentRepo;
        private readonly IItineraryStopRepository _stopRepo;
        private readonly IProviderDepositRepository _depositRepo;
        private readonly IProviderUserRepository _providerUserRepo;
        private readonly IUserRepository _userRepo;
        private readonly IBookingRepository _bookingRepository;
        private readonly IWishListRepository _wishListRepo;

        private static readonly DateTime ActivityBaseDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const double DepositRate = 0.20;

        public ItineraryScheduleService(
            IItineraryScheduleRepository repo,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IBookingItineraryRepository bookingItineraryRepository,
            INotificationRepository notificationRepo,
            IAgencyUserRepository agencyUserRepo,
            IStopStaffAssignmentRepository assignmentRepo,
            IItineraryStopRepository stopRepo,
            IProviderDepositRepository depositRepo,
            IProviderUserRepository providerUserRepo,
            IUserRepository userRepo,
            IBookingRepository bookingRepository,
            IWishListRepository wishListRepo
        )
        {
            _repo = repo;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _bookingItineraryRepository = bookingItineraryRepository;
            _notificationRepo = notificationRepo;
            _agencyUserRepo   = agencyUserRepo;
            _assignmentRepo   = assignmentRepo;
            _stopRepo         = stopRepo;
            _depositRepo      = depositRepo;
            _providerUserRepo = providerUserRepo;
            _userRepo         = userRepo;
            _bookingRepository = bookingRepository;
            _wishListRepo      = wishListRepo;
        }

        public async Task<List<ItineraryScheduleDTO>> GetByItineraryIdAsync(Guid itineraryId)
        {
            var list = await _repo.GetByItineraryIdAsync(itineraryId);
            return list.Select(MapToDTO).ToList();
        }

        public async Task<ItineraryScheduleDTO> AddAsync(Guid itineraryId, ItineraryScheduleCreateRequest request, Guid agencyId)
        {
            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.");
            if (request.Spot < 1)
                throw new ArgumentException("Spot must be at least 1.");

            // ── Calculate required deposit (20% of provider activity totals) ─────
            var stops         = await _stopRepo.GetWithProviderAndActivitiesByItineraryIdAsync(itineraryId);
            var depositItems  = new List<(Guid providerId, string providerName, Guid stopId, long serviceTotal, long depositAmt, DateTime actualTime)>();

            foreach (var stop in stops.Where(s => s.ProviderId.HasValue && s.Activities.Any()))
            {
                long serviceTotal = stop.Activities.Sum(a => a.Price);
                if (serviceTotal <= 0) continue;

                var first      = stop.Activities.OrderBy(a => a.StartTime).First();
                // Activities saved with BASE_DATE (year 2000) encode day offset.
                // If stored with a different year (old data), treat as Day 1 (offset = 0).
                int dayOffset  = first.StartTime.Year == 2000
                    ? (int)(first.StartTime.Date - ActivityBaseDate.Date).TotalDays
                    : 0;
                var actualTime = request.StartTime.Date.AddDays(dayOffset).Add(first.StartTime.TimeOfDay);

                depositItems.Add((
                    stop.ProviderId!.Value,
                    stop.Provider?.Name ?? "Provider",
                    stop.Id,
                    serviceTotal * request.Spot,                              // total for all expected pax
                    (long)Math.Ceiling(serviceTotal * request.Spot * DepositRate), // 20% × total
                    actualTime
                ));
            }

            long totalDeposit = depositItems.Sum(d => d.depositAmt);

            // ── Deduct deposit from agency wallet ────────────────────────────────
            if (totalDeposit > 0)
            {
                var agencyManager = await _agencyUserRepo.GetManagerByAgencyIdAsync(agencyId)
                    ?? throw new InvalidOperationException("Agency manager not found. Please top up first.");

                var wallet = await _walletRepository.GetByUserIdAsync(agencyManager.UserId)
                    ?? throw new InvalidOperationException("Agency manager wallet not found. Please top up first.");

                if (wallet.Balance < totalDeposit)
                    throw new InvalidOperationException(
                        $"Insufficient wallet balance. Required: {totalDeposit:N0}đ, Available: {wallet.Balance:N0}đ");

                wallet.Balance   -= totalDeposit;
                wallet.UpdatedAt  = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await _walletTransactionRepository.CreateAsync(new WalletTransaction
                {
                    Id          = Guid.NewGuid(),
                    WalletId    = wallet.Id,
                    Amount      = totalDeposit,
                    Type        = WalletTransactionType.Debit,
                    Reason      = WalletTransactionReason.BookingEarning,
                    Note        = $"Đặt cọc 20% cho lịch {itineraryId} ({request.StartTime:dd/MM/yyyy})",
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow,
                });
            }

            // ── Create schedule ───────────────────────────────────────────────────
            var schedule = new ItinerarySchedule
            {
                Id          = Guid.NewGuid(),
                ItineraryId = itineraryId,
                StartTime   = request.StartTime,
                EndTime     = request.EndTime,
                Spot        = request.Spot,
                SpotLeft    = request.Spot,
                GuideId     = request.GuideId,
                Status      = ItineraryScheduleStatus.Pending,
                CreatedAt   = DateTime.UtcNow,
            };
            var saved = await _repo.CreateAsync(schedule);

            // ── Create ProviderDeposit records ────────────────────────────────────
            foreach (var (providerId, providerName, stopId, serviceTotal, depositAmt, actualTime) in depositItems)
            {
                await _depositRepo.CreateAsync(new Domain.Entities.ProviderDeposit
                {
                    Id                      = Guid.NewGuid(),
                    ItineraryScheduleId     = saved.Id,
                    ProviderId              = providerId,
                    ProviderName            = providerName,
                    ItineraryStopId         = stopId,
                    ActualFirstActivityTime = actualTime,
                    ServiceTotal            = serviceTotal,
                    DepositAmount           = depositAmt,
                    Status                  = Domain.Enums.DepositStatus.Paid,
                    CreatedAt               = DateTime.UtcNow,
                    UpdatedAt               = DateTime.UtcNow,
                });
            }

            // Load full details for notification + return value
            var full = await _repo.GetScheduleWithDetails(saved.Id);

            // Notify guide if assigned
            if (request.GuideId.HasValue && full?.Itinerary != null)
            {
                var itineraryName = full.Itinerary.Name;
                var startDate     = request.StartTime.ToString("dd MMM yyyy");
                var endDate       = request.EndTime.ToString("dd MMM yyyy");

                await _notificationRepo.CreateAsync(new Notification
                {
                    RecipientUserId = request.GuideId.Value,
                    Title           = "New Job Assignment",
                    Message         = $"You have been assigned as tour guide for \"{itineraryName}\" ({startDate} – {endDate}). Please go to your Jobs page to accept or reject.",
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = saved.Id,
                });
            }

            return MapToDTO(full ?? saved);
        }

        private static ItineraryScheduleDTO MapToDTO(ItinerarySchedule s) => new()
        {
            Id          = s.Id,
            ItineraryId = s.ItineraryId,
            StartTime   = s.StartTime,
            EndTime     = s.EndTime,
            Spot        = s.Spot,
            SpotLeft    = s.SpotLeft,
            GuideId     = s.GuideId,
            GuideName   = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
        };

        private static AgencyScheduleDTO MapToAgencyDTO(ItinerarySchedule s)
        {
            // First activity across all stops, ordered by stored StartTime (BASE_DATE + time).
            // Return HH:mm only — avoids timezone conversion issues on the frontend.
            var firstAct = s.Itinerary?.Stops
                .SelectMany(st => st.Activities)
                .OrderBy(a => a.StartTime)
                .FirstOrDefault();

            return new AgencyScheduleDTO
            {
                Id                  = s.Id,
                ItineraryId         = s.ItineraryId,
                ItineraryName       = s.Itinerary?.Name ?? string.Empty,
                StartTime           = s.StartTime,
                EndTime             = s.EndTime,
                Spot                = s.Spot,
                SpotLeft            = s.SpotLeft,
                GuideId             = s.GuideId,
                GuideName           = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
                Status              = s.Status.ToString(),
                FirstActivityTime   = firstAct?.StartTime.ToString("HH:mm"),
            };
        }

        private static AdminScheduleDTO MapToAdminDTO(ItinerarySchedule s)
        {
            var agency = MapToAgencyDTO(s);
            return new AdminScheduleDTO
            {
                Id                = agency.Id,
                ItineraryId       = agency.ItineraryId,
                ItineraryName     = agency.ItineraryName,
                AgencyId          = s.Itinerary?.AgencyId ?? Guid.Empty,
                AgencyName        = s.Itinerary?.Agency?.Name ?? string.Empty,
                StartTime         = agency.StartTime,
                EndTime           = agency.EndTime,
                Spot              = agency.Spot,
                SpotLeft          = agency.SpotLeft,
                GuideId           = agency.GuideId,
                GuideName         = agency.GuideName,
                Status            = agency.Status,
                FirstActivityTime = agency.FirstActivityTime,
            };
        }

        public async Task<List<AdminScheduleDTO>> GetAllSchedulesAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToAdminDTO).ToList();
        }

        public async Task<List<AgencyScheduleDTO>> GetByAgencyIdAsync(Guid agencyId)
        {
            var list = await _repo.GetByAgencyIdAsync(agencyId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<AgencyScheduleDTO>> GetByGuideIdAsync(Guid guideId)
        {
            var list = await _repo.GetByGuideIdAsync(guideId);
            return list.Select(MapToAgencyDTO).ToList();
        }

        public async Task<List<ProviderScheduleDTO>> GetByProviderIdAsync(Guid providerId)
        {
            var list = await _repo.GetByProviderIdAsync(providerId);
            return list.Select(s =>
            {
                var firstActivity = s.Itinerary?.Stops
                    .Where(st => st.ProviderId == providerId)
                    .SelectMany(st => st.Activities)
                    .OrderBy(a => a.StartTime)
                    .FirstOrDefault();

                return new ProviderScheduleDTO
                {
                    Id                = s.Id,
                    ItineraryId       = s.ItineraryId,
                    ItineraryName     = s.Itinerary?.Name ?? string.Empty,
                    AgencyName        = s.Itinerary?.Agency?.Name ?? string.Empty,
                    StartTime         = s.StartTime,
                    EndTime           = s.EndTime,
                    Spot              = s.Spot,
                    SpotLeft          = s.SpotLeft,
                    GuideId           = s.GuideId,
                    GuideName         = s.Guide != null ? (s.Guide.FullName ?? s.Guide.Username) : null,
                    FirstActivityTime = firstActivity?.StartTime,
                };
            }).ToList();
        }

        public async Task<List<ProviderJobWithStopsDTO>> GetSchedulesWithStopsAsync(Guid providerId)
        {
            var list = await _repo.GetByProviderIdAsync(providerId);

            // Load all assignments for these schedules in one query
            var scheduleIds = list.Select(s => s.Id).ToList();
            var allAssignments = await _assignmentRepo.GetByScheduleIdsAsync(scheduleIds);

            return list.Select(s =>
            {
                var assignmentMap = allAssignments
                    .Where(a => a.ScheduleId == s.Id)
                    .ToDictionary(a => a.StopId);

                return new ProviderJobWithStopsDTO
                {
                    ScheduleId    = s.Id,
                    ItineraryId   = s.ItineraryId,
                    ItineraryName = s.Itinerary?.Name ?? string.Empty,
                    AgencyName    = s.Itinerary?.Agency?.Name ?? string.Empty,
                    StartTime     = s.StartTime,
                    EndTime       = s.EndTime,
                    Spot          = s.Spot,
                    SpotLeft      = s.SpotLeft,
                    Status        = s.Status.ToString(),
                    Stops         = (s.Itinerary?.Stops ?? [])
                        .Where(st => st.ProviderId == providerId)
                        .OrderBy(st => st.StopOrder)
                        .Select(st =>
                        {
                            assignmentMap.TryGetValue(st.Id, out var asgn);
                            return new ProviderStopDetailDTO
                            {
                                StopId             = st.Id,
                                Name               = st.Name,
                                StopOrder          = st.StopOrder,
                                Address            = st.Address,
                                Latitude           = st.Latitude,
                                Longitude          = st.Longitude,
                                AssignedStaffId    = asgn?.StaffId,
                                AssignedStaffName  = asgn?.Staff != null ? (asgn.Staff.FullName ?? asgn.Staff.Username) : null,
                                AssignedStaffEmail = asgn?.Staff?.Email,
                                Activities         = st.Activities
                                    .OrderBy(a => a.ActivityOrder)
                                    .Select(a => new StopActivityDTO
                                    {
                                        ActivityId    = a.Id,
                                        ActivityOrder = a.ActivityOrder,
                                        Name          = a.CustomName ?? a.Service?.Name ?? "Activity",
                                        StartTime     = a.StartTime,
                                        EndTime       = a.EndTime,
                                        Price         = a.Price,
                                        Note          = a.Note,
                                    }).ToList(),
                            };
                        }).ToList()
                };
            }).ToList();
        }

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            return await _repo.DeleteAsync(scheduleId);
        }

        public async Task AcceptScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be accepted");

            schedule.Status    = ItineraryScheduleStatus.Confirmed;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            // Notify all agency managers
            await NotifyManagersAsync(scheduleId,
                title:   "Schedule Confirmed",
                message: s => $"Your guide has accepted the schedule for \"{s.Itinerary?.Name}\". The tour is now confirmed.");

            // Notify users who have this itinerary on their wishlist
            await NotifyWishlistUsersOnScheduleConfirmedAsync(scheduleId);
        }

        public async Task RejectScheduleAsync(Guid scheduleId, Guid guideId)
        {
            var schedule = await _repo.GetByIdAsync(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.GuideId != guideId)
                throw new UnauthorizedAccessException("You are not assigned to this schedule");

            if (schedule.Status != ItineraryScheduleStatus.Pending)
                throw new InvalidOperationException("Only pending schedules can be rejected");

            schedule.GuideId   = null;
            schedule.Status    = ItineraryScheduleStatus.Pending;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            // Notify all agency managers
            await NotifyManagersAsync(scheduleId,
                title:   "Schedule Rejected",
                message: s => $"Your guide has declined the schedule for \"{s.Itinerary?.Name}\". Please assign a new guide.");
        }

        private async Task NotifyWishlistUsersOnScheduleConfirmedAsync(Guid scheduleId)
        {
            var schedule = await _repo.GetScheduleWithDetails(scheduleId);
            if (schedule == null) return;

            var wishlistEntries = await _wishListRepo.GetByItineraryIdAsync(schedule.ItineraryId);
            if (!wishlistEntries.Any()) return;

            var tourName  = schedule.Itinerary?.Name ?? schedule.ItineraryId.ToString();
            var dateRange = $"{schedule.StartTime:dd/MM/yyyy} – {schedule.EndTime:dd/MM/yyyy}";

            foreach (var entry in wishlistEntries)
            {
                await _notificationRepo.CreateAsync(new Notification
                {
                    RecipientUserId = entry.UserId,
                    Title           = "Lịch mới vừa mở đặt chỗ!",
                    Message         = $"Tour \"{tourName}\" bạn yêu thích vừa có lịch mới ({dateRange}). Đặt ngay trước khi hết chỗ!",
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = schedule.ItineraryId,
                });
            }
        }

        private async Task NotifyManagersAsync(Guid scheduleId, string title, Func<ItinerarySchedule, string> message)
        {
            var details = await _repo.GetScheduleWithDetails(scheduleId);
            if (details?.Itinerary == null) return;

            var agencyUsers = await _agencyUserRepo.GetAgencyUsers(details.Itinerary.AgencyId);
            var managers    = agencyUsers.Where(u => u.Role == AgencyUserRole.Manager);

            foreach (var manager in managers)
            {
                await _notificationRepo.CreateAsync(new Notification
                {
                    RecipientUserId = manager.UserId,
                    Title           = title,
                    Message         = message(details),
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = scheduleId,
                });
            }
        }
        public async Task UpdateStatusAsync(Guid scheduleId, ItineraryScheduleStatus status)
        {
            var schedule = await _repo.GetWithStopsAndActivitiesAsync(scheduleId);
            if (schedule == null) throw new KeyNotFoundException("Schedule not found");

            var wasAlreadyCompleted = schedule.Status == ItineraryScheduleStatus.Completed;

            schedule.Status = status;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _repo.UpdateAsync(schedule);

            if (status != ItineraryScheduleStatus.Completed || wasAlreadyCompleted) return;

            // Return provider deposits to agency wallet since tour completed normally
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var paidDeposits = deposits.Where(d => d.Status == Domain.Enums.DepositStatus.Paid).ToList();
            if (paidDeposits.Any())
            {
                long refundTotal = paidDeposits.Sum(d => d.DepositAmount);
                foreach (var dep in paidDeposits) dep.Status = Domain.Enums.DepositStatus.Returned;
                await _depositRepo.UpdateRangeAsync(paidDeposits);

                var agencyManager = await _agencyUserRepo.GetManagerByAgencyIdAsync(schedule.Itinerary.AgencyId);
                if (agencyManager != null)
                {
                    var agencyWallet = await _walletRepository.GetByUserIdAsync(agencyManager.UserId)
                        ?? await _walletRepository.CreateAsync(new Wallet
                        {
                            Id = Guid.NewGuid(),
                            UserId = agencyManager.UserId,
                            Balance = 0,
                            PendingBalance = 0,
                            CreatedAt = DateTime.UtcNow,
                        });

                    agencyWallet.Balance   += refundTotal;
                    agencyWallet.UpdatedAt  = DateTime.UtcNow;
                    await _walletRepository.UpdateAsync(agencyWallet);

                    await _walletTransactionRepository.CreateAsync(new WalletTransaction
                    {
                        Id          = Guid.NewGuid(),
                        WalletId    = agencyWallet.Id,
                        Amount      = refundTotal,
                        Type        = WalletTransactionType.Credit,
                        Reason      = WalletTransactionReason.Refund,
                        ReferenceId = scheduleId,
                        Note        = "Hoàn tiền cọc sau khi tour hoàn thành",
                        CreatedAt   = DateTime.UtcNow,
                        UpdatedAt   = DateTime.UtcNow,
                    });
                }
            }

            // ── Payout formulas ───────────────────────────────────────────────
            // provider_payout = provider_service_per_pax × actual_pax × 80%
            // agency_payout   = agency_service_per_pax  × actual_pax × 80%
            //                   (deposit already returned in the block above)
            // admin keeps     = total_trip × actual_pax × 20%

            const double PayoutRate = 0.80;

            // Sum provider activity prices per pax (per provider and in total)
            var providerStops = schedule.Itinerary.Stops
                .Where(s => s.ProviderId.HasValue)
                .ToList();

            long totalProviderPerPax = providerStops
                .SelectMany(s => s.Activities)
                .Sum(a => a.Price);

            // actual pax = sum of non-cancelled booking guests
            var bookingItineraries = await _bookingItineraryRepository.GetByScheduleIdAsync(scheduleId);
            var activeBookingLines = bookingItineraries
                .Where(bi => bi.Status != BookingItineraryStatus.Cancelled)
                .ToList();
            int actualPax = activeBookingLines.Sum(bi => bi.NumberOfGuests);

            foreach (var line in activeBookingLines)
            {
                line.Status    = BookingItineraryStatus.Completed;
                line.UpdatedAt = DateTime.UtcNow;
                await _bookingItineraryRepository.UpdateAsync(line);
            }

            var bookingIds = activeBookingLines.Select(bi => bi.BookingId).Distinct();
            foreach (var bookingId in bookingIds)
            {
                var booking = await _bookingRepository.GetByIdAsync(bookingId);
                if (booking == null) continue;

                var allLines = await _bookingItineraryRepository.GetBookingItinerariesByBookingId(bookingId);
                if (allLines.All(l => l.Status is BookingItineraryStatus.Completed or BookingItineraryStatus.Cancelled))
                {
                    booking.Status    = BookingStatus.Completed;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingRepository.UpdateAsync(booking);
                }
            }

            if (actualPax <= 0) return;

            // Credit each provider: their per-pax service total × actual_pax × 80%
            var providerGroups = providerStops
                .GroupBy(s => s.ProviderId!.Value)
                .Select(g => new
                {
                    ProviderId    = g.Key,
                    PerPaxTotal   = g.SelectMany(s => s.Activities).Sum(a => a.Price)
                });

            foreach (var pg in providerGroups)
            {
                long providerPayout = (long)(pg.PerPaxTotal * actualPax * PayoutRate);
                if (providerPayout <= 0) continue;

                var providerManagers = await _providerUserRepo.GetManagersByProviderIdAsync(pg.ProviderId);
                var providerManager = providerManagers.FirstOrDefault();
                if (providerManager == null) continue;

                var wallet = await _walletRepository.GetByUserIdAsync(providerManager.UserId)
                    ?? await _walletRepository.CreateAsync(new Wallet
                    {
                        Id = Guid.NewGuid(),
                        UserId = providerManager.UserId,
                        Balance = 0,
                        PendingBalance = 0,
                        CreatedAt = DateTime.UtcNow,
                    });

                wallet.Balance   += providerPayout;
                wallet.UpdatedAt  = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await _walletTransactionRepository.CreateAsync(new WalletTransaction
                {
                    Id          = Guid.NewGuid(),
                    WalletId    = wallet.Id,
                    Amount      = providerPayout,
                    Type        = WalletTransactionType.Credit,
                    Reason      = WalletTransactionReason.BookingEarning,
                    ReferenceId = scheduleId,
                    Note        = $"Provider earnings: {pg.PerPaxTotal:N0}đ/pax × {actualPax} pax × 80%",
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow,
                });

                // Notify provider managers
                foreach (var manager in providerManagers)
                {
                    await _notificationRepo.CreateAsync(new Notification
                    {
                        Id              = Guid.NewGuid(),
                        RecipientUserId = manager.UserId,
                        Title           = "Tour Completed — Earnings Received",
                        Message         = $"Tour \"{schedule.Itinerary.Name}\" completed. {providerPayout:N0}đ ({pg.PerPaxTotal:N0}đ/pax × {actualPax} pax × 80%) has been credited to the provider manager wallet.",
                        EntityType      = NotificationEntityType.Itinerary,
                        EntityId        = scheduleId,
                        IsRead          = false,
                        CreatedAt       = DateTime.UtcNow,
                    });
                }
            }

            // Credit agency: agency_service_per_pax × actual_pax × 80%
            // agency_service_per_pax = itinerary.Price − total_provider_per_pax
            long agencyPerPax   = schedule.Itinerary.Price - totalProviderPerPax;
            long agencyPayout   = (long)(agencyPerPax * actualPax * PayoutRate);

            if (agencyPayout > 0)
            {
                var agencyManager = await _agencyUserRepo.GetManagerByAgencyIdAsync(schedule.Itinerary.AgencyId);
                if (agencyManager != null)
                {
                    var agencyWallet = await _walletRepository.GetByUserIdAsync(agencyManager.UserId)
                        ?? await _walletRepository.CreateAsync(new Wallet
                        {
                            Id = Guid.NewGuid(),
                            UserId = agencyManager.UserId,
                            Balance = 0,
                            PendingBalance = 0,
                            CreatedAt = DateTime.UtcNow,
                        });

                    agencyWallet.Balance   += agencyPayout;
                    agencyWallet.UpdatedAt  = DateTime.UtcNow;
                    await _walletRepository.UpdateAsync(agencyWallet);

                    await _walletTransactionRepository.CreateAsync(new WalletTransaction
                    {
                        Id          = Guid.NewGuid(),
                        WalletId    = agencyWallet.Id,
                        Amount      = agencyPayout,
                        Type        = WalletTransactionType.Credit,
                        Reason      = WalletTransactionReason.BookingEarning,
                        ReferenceId = scheduleId,
                        Note        = $"Agency earnings: {agencyPerPax:N0}đ/pax × {actualPax} pax × 80%",
                        CreatedAt   = DateTime.UtcNow,
                        UpdatedAt   = DateTime.UtcNow,
                    });

                    // Notify agency managers
                    var depositTotal = paidDeposits.Sum(d => d.DepositAmount);
                    var agencyUsers  = await _agencyUserRepo.GetAgencyUsers(schedule.Itinerary.AgencyId);
                    foreach (var manager in agencyUsers.Where(u => u.Role == AgencyUserRole.Manager))
                    {
                        await _notificationRepo.CreateAsync(new Notification
                        {
                            Id              = Guid.NewGuid(),
                            RecipientUserId = manager.UserId,
                            Title           = "Tour Completed — Earnings & Deposit Received",
                            Message         = $"Tour \"{schedule.Itinerary.Name}\" completed. " +
                                              $"Earnings {agencyPayout:N0}đ + deposit return {depositTotal:N0}đ " +
                                              $"= {agencyPayout + depositTotal:N0}đ credited to your wallet.",
                            EntityType      = NotificationEntityType.Itinerary,
                            EntityId        = scheduleId,
                            IsRead          = false,
                            CreatedAt       = DateTime.UtcNow,
                        });
                    }
                }
            }

            // Credit admin accounts: 20% commission on total trip price × actual pax
            long totalAdminCommission = (long)(schedule.Itinerary.Price * actualPax * (1 - PayoutRate));
            if (totalAdminCommission > 0)
            {
                var adminUsers = await _userRepo.GetAllByRoleCodeAsync("ADMIN");
                if (adminUsers.Count > 0)
                {
                    long perAdminShare = totalAdminCommission / adminUsers.Count;
                    foreach (var admin in adminUsers)
                    {
                        var adminWallet = await _walletRepository.GetByUserIdAsync(admin.Id);
                        if (adminWallet == null) continue;

                        adminWallet.Balance  += perAdminShare;
                        adminWallet.UpdatedAt = DateTime.UtcNow;
                        await _walletRepository.UpdateAsync(adminWallet);

                        await _walletTransactionRepository.CreateAsync(new WalletTransaction
                        {
                            Id          = Guid.NewGuid(),
                            WalletId    = adminWallet.Id,
                            Amount      = perAdminShare,
                            Type        = WalletTransactionType.Credit,
                            Reason      = WalletTransactionReason.Commission,
                            ReferenceId = scheduleId,
                            Note        = $"Platform commission: {schedule.Itinerary.Price:N0}đ/pax × {actualPax} pax × 20% ÷ {adminUsers.Count} admin",
                            CreatedAt   = DateTime.UtcNow,
                            UpdatedAt   = DateTime.UtcNow,
                        });

                        await _notificationRepo.CreateAsync(new Notification
                        {
                            Id              = Guid.NewGuid(),
                            RecipientUserId = admin.Id,
                            Title           = "Tour Completed — Commission Received",
                            Message         = $"Tour \"{schedule.Itinerary.Name}\" completed. Platform commission {perAdminShare:N0}đ has been credited to your wallet.",
                            EntityType      = NotificationEntityType.Itinerary,
                            EntityId        = scheduleId,
                            IsRead          = false,
                            CreatedAt       = DateTime.UtcNow,
                        });
                    }
                }
            }
        }

    }
}

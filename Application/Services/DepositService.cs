using TouRest.Application.DTOs.Deposit;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Enums;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class DepositService : IDepositService
    {
        private readonly IProviderDepositRepository _depositRepo;
        private readonly IBookingItineraryRepository _biRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IItineraryScheduleRepository _scheduleRepo;
        private readonly IItineraryStopRepository _stopRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _walletTxRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IRefundRepository _refundRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IAgencyUserRepository _agencyUserRepo;
        private readonly IProviderUserRepository _providerUserRepo;

        private static readonly DateTime ActivityBaseDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const double DepositRate = 0.20;
        private const int CancelWindowHours = 48;

        public DepositService(
            IProviderDepositRepository depositRepo,
            IBookingItineraryRepository biRepo,
            IBookingRepository bookingRepo,
            IItineraryScheduleRepository scheduleRepo,
            IItineraryStopRepository stopRepo,
            IWalletRepository walletRepo,
            IWalletTransactionRepository walletTxRepo,
            IPaymentRepository paymentRepo,
            IRefundRepository refundRepo,
            INotificationRepository notificationRepo,
            IAgencyUserRepository agencyUserRepo,
            IProviderUserRepository providerUserRepo)
        {
            _depositRepo      = depositRepo;
            _biRepo           = biRepo;
            _bookingRepo      = bookingRepo;
            _scheduleRepo     = scheduleRepo;
            _stopRepo         = stopRepo;
            _walletRepo       = walletRepo;
            _walletTxRepo     = walletTxRepo;
            _paymentRepo      = paymentRepo;
            _refundRepo       = refundRepo;
            _notificationRepo = notificationRepo;
            _agencyUserRepo   = agencyUserRepo;
            _providerUserRepo = providerUserRepo;
        }

        public async Task<DepositCalculationDTO> CalculateAsync(Guid itineraryId, DateTime scheduleStartTime)
        {
            var stops  = await _stopRepo.GetWithProviderAndActivitiesByItineraryIdAsync(itineraryId);
            var result = new DepositCalculationDTO { ItineraryId = itineraryId };

            foreach (var stop in stops.Where(s => s.ProviderId.HasValue && s.Activities.Any()))
            {
                long serviceTotal = stop.Activities.Sum(a => a.Price);
                if (serviceTotal <= 0) continue;

                var first     = stop.Activities.OrderBy(a => a.StartTime).First();
                int dayOffset = first.StartTime.Year == 2000
                    ? (int)(first.StartTime.Date - ActivityBaseDate.Date).TotalDays
                    : 0;
                var actualTime = scheduleStartTime.Date.AddDays(dayOffset).Add(first.StartTime.TimeOfDay);

                result.Providers.Add(new ProviderDepositDTO
                {
                    ProviderId              = stop.ProviderId!.Value,
                    ProviderName            = stop.Provider?.Name ?? "Provider",
                    ActualFirstActivityTime = actualTime,
                    ServiceTotal            = serviceTotal,
                    DepositAmount           = (long)Math.Ceiling(serviceTotal * DepositRate),
                    Status                  = "Preview",
                });
            }

            result.TotalDepositAmount = result.Providers.Sum(p => p.DepositAmount);
            return result;
        }

        public async Task<BookingDepositSummaryDTO> GetByScheduleAsync(Guid scheduleId)
        {
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            return new BookingDepositSummaryDTO
            {
                ScheduleId         = scheduleId,
                TotalDepositAmount = deposits.Sum(d => d.DepositAmount),
                ProviderDeposits   = deposits.Select(MapDTO).ToList(),
            };
        }

        public async Task<ScheduleCancelPreviewDTO> PreviewCancelAsync(Guid scheduleId)
        {
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var now      = DateTime.UtcNow;
            var details  = BuildDetails(deposits, now);
            var (affectedBookings, _) = await CalculateCustomerRefundPreviewAsync(scheduleId);

            return new ScheduleCancelPreviewDTO
            {
                ScheduleId        = scheduleId,
                AffectedBookings  = affectedBookings,
                TotalToRefund     = details.Where(d => d.Outcome == "Refunded").Sum(d => d.DepositAmount),
                TotalToForfeit    = details.Where(d => d.Outcome == "Forfeited").Sum(d => d.DepositAmount),
                Details           = details,
            };
        }

        public async Task<AdminScheduleCancelPreviewDTO> PreviewAdminCancelAsync(Guid scheduleId)
        {
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var (affectedBookings, totalCustomerRefund) = await CalculateCustomerRefundPreviewAsync(scheduleId);

            return new AdminScheduleCancelPreviewDTO
            {
                ScheduleId           = scheduleId,
                AffectedBookings     = affectedBookings,
                TotalDepositRefund   = deposits.Where(d => d.Status == DepositStatus.Paid).Sum(d => d.DepositAmount),
                TotalCustomerRefund  = totalCustomerRefund,
            };
        }

        public async Task<AgencyCancelResultDTO> AgencyCancelScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetCancellableScheduleAsync(scheduleId);
            var agencyId = schedule.Itinerary?.AgencyId
                ?? throw new InvalidOperationException("Cannot determine agency for this schedule");

            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var now      = DateTime.UtcNow;
            var result   = new AgencyCancelResultDTO { ScheduleId = scheduleId };
            var toUpdate = new List<ProviderDeposit>();

            foreach (var deposit in deposits.Where(d => d.Status == DepositStatus.Paid))
            {
                var threshold = deposit.ActualFirstActivityTime.AddHours(-CancelWindowHours);

                if (now < threshold)
                {
                    deposit.Status = DepositStatus.Refunded;
                    toUpdate.Add(deposit);

                    await CreditAgencyWalletAsync(
                        agencyId,
                        deposit.DepositAmount,
                        scheduleId,
                        $"Hoàn cọc provider {deposit.ProviderName} (hủy trước 48h)");

                    result.TotalRefunded += deposit.DepositAmount;
                    result.Details.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Refunded",
                        Reason                  = $"Cancelled more than {CancelWindowHours}h before service",
                    });
                }
                else
                {
                    deposit.Status = DepositStatus.Forfeited;
                    toUpdate.Add(deposit);

                    result.TotalForfeited += deposit.DepositAmount;
                    result.Details.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Forfeited",
                        Reason                  = $"Cancelled within {CancelWindowHours}h of service",
                    });
                }
            }

            if (toUpdate.Any())
                await _depositRepo.UpdateRangeAsync(toUpdate);

            var tourName = schedule.Itinerary?.Name ?? scheduleId.ToString();
            await CancelBookingsAndScheduleAsync(schedule, scheduleId, tourName, result, cancelledByAdmin: false);
            await NotifyAgencyOnScheduleCancelledAsync(schedule, agencyId, scheduleId, tourName, result, cancelledByAdmin: false);
            await NotifyProvidersOnScheduleCancelledAsync(schedule.ItineraryId, scheduleId, tourName, cancelledByAdmin: false);

            return result;
        }

        public async Task<AgencyCancelResultDTO> AdminCancelScheduleAsync(Guid scheduleId)
        {
            var schedule = await GetCancellableScheduleAsync(scheduleId);
            var agencyId = schedule.Itinerary?.AgencyId
                ?? throw new InvalidOperationException("Cannot determine agency for this schedule");

            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var result   = new AgencyCancelResultDTO { ScheduleId = scheduleId };
            var toUpdate = new List<ProviderDeposit>();

            foreach (var deposit in deposits.Where(d => d.Status == DepositStatus.Paid))
            {
                deposit.Status = DepositStatus.Refunded;
                toUpdate.Add(deposit);

                await CreditAgencyWalletAsync(
                    agencyId,
                    deposit.DepositAmount,
                    scheduleId,
                    $"Hoàn cọc provider {deposit.ProviderName} (hủy bởi admin)");

                result.TotalRefunded += deposit.DepositAmount;
                result.Details.Add(new DepositCancelDetail
                {
                    ProviderName            = deposit.ProviderName,
                    DepositAmount           = deposit.DepositAmount,
                    ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                    Outcome                 = "Refunded",
                    Reason                  = "Admin cancellation — full deposit refund",
                });
            }

            if (toUpdate.Any())
                await _depositRepo.UpdateRangeAsync(toUpdate);

            var tourName = schedule.Itinerary?.Name ?? scheduleId.ToString();
            await CancelBookingsAndScheduleAsync(schedule, scheduleId, tourName, result, cancelledByAdmin: true);
            await NotifyAgencyOnScheduleCancelledAsync(schedule, agencyId, scheduleId, tourName, result, cancelledByAdmin: true);
            await NotifyProvidersOnScheduleCancelledAsync(schedule.ItineraryId, scheduleId, tourName, cancelledByAdmin: true);

            return result;
        }

        /// <summary>
        /// Preview what will happen if agency cancels an ONGOING schedule now (no side effects).
        /// </summary>
        public async Task<OngoingCancelPreviewDTO> PreviewOngoingCancelAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepo.GetScheduleWithDetails(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.Status != ItineraryScheduleStatus.Ongoing)
                throw new InvalidOperationException("Schedule is not ongoing.");

            var itinerary = schedule.Itinerary!;
            var now       = DateTime.UtcNow;

            var biLines     = await _biRepo.GetByScheduleIdAsync(scheduleId);
            var activeLines = biLines.Where(l => l.Status != BookingItineraryStatus.Cancelled).ToList();
            int actualPax   = activeLines.Sum(l => l.NumberOfGuests);

            var stops         = await _stopRepo.GetWithProviderAndActivitiesByItineraryIdAsync(schedule.ItineraryId);
            var allActivities = stops.SelectMany(s => s.Activities).ToList();
            long totalActivitiesPrice     = allActivities.Sum(a => a.Price);
            long completedActivitiesPrice = 0;

            foreach (var act in allActivities)
            {
                int dayOffset  = act.StartTime.Year == 2000 ? (int)(act.StartTime.Date - ActivityBaseDate.Date).TotalDays : 0;
                var actEndTime = schedule.StartTime.Date.AddDays(dayOffset).Add(act.EndTime.TimeOfDay);
                if (actEndTime < now)
                    completedActivitiesPrice += act.Price;
            }

            double completionRatio = totalActivitiesPrice > 0
                ? (double)completedActivitiesPrice / totalActivitiesPrice
                : Math.Clamp((now - schedule.StartTime).TotalSeconds / (schedule.EndTime - schedule.StartTime).TotalSeconds, 0, 1);

            var deposits                  = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            long depositsReturnedToAgency = 0;
            long depositsKeptByProviders  = 0;
            var  depositDetails           = new List<DepositCancelDetail>();

            foreach (var deposit in deposits.Where(d => d.Status == DepositStatus.Paid))
            {
                if (deposit.ActualFirstActivityTime <= now)
                {
                    depositsReturnedToAgency += deposit.DepositAmount;
                    depositDetails.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Returned",
                        Reason                  = "Service already completed",
                    });
                }
                else if (now < deposit.ActualFirstActivityTime.AddHours(-CancelWindowHours))
                {
                    depositsReturnedToAgency += deposit.DepositAmount;
                    depositDetails.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Refunded",
                        Reason                  = $"More than {CancelWindowHours}h before service",
                    });
                }
                else
                {
                    depositsKeptByProviders += deposit.DepositAmount;
                    depositDetails.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Forfeited",
                        Reason                  = $"Within {CancelWindowHours}h of service — provider keeps",
                    });
                }
            }

            long totalCustomerRefund = 0;
            var  processedBookings   = new HashSet<Guid>();
            foreach (var line in activeLines)
            {
                if (!processedBookings.Add(line.BookingId)) continue;
                var payment = await _paymentRepo.GetPaidPaymentByBookingIdAsync(line.BookingId);
                if (payment == null || payment.FinalAmount <= 0) continue;
                long refundAmount = payment.FinalAmount - (long)(payment.FinalAmount * completionRatio);
                if (refundAmount > 0)
                    totalCustomerRefund += refundAmount;
            }

            return new OngoingCancelPreviewDTO
            {
                ScheduleId               = scheduleId,
                CompletionRatioPercent   = Math.Round(completionRatio * 100, 2),
                AffectedBookings         = processedBookings.Count,
                ActualPassengers         = actualPax,
                AgencyServiceEarnings    = (long)(itinerary.Price * completionRatio * actualPax),
                DepositsReturnedToAgency = depositsReturnedToAgency,
                DepositsKeptByProviders  = depositsKeptByProviders,
                TotalCustomerRefund      = totalCustomerRefund,
                DepositDetails           = depositDetails,
            };
        }

        /// <summary>
        /// Agency cancels a schedule while it is Ongoing.
        ///
        /// Agency receives:
        ///   - tour price × completion ratio × actual pax  (value of completed services they own)
        ///   - deposits for completed services and upcoming services &gt;48h (returned/refunded)
        ///
        /// Provider "khám" receives:
        ///   - deposit for any upcoming service ≤48h (kept as cancellation penalty)
        ///
        /// Customer receives:
        ///   - amount paid − used services value  (payment.FinalAmount × completion ratio)
        /// </summary>
        public async Task<AgencyCancelResultDTO> AgencyCancelOngoingScheduleAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepo.GetScheduleWithDetails(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.Status != ItineraryScheduleStatus.Ongoing)
                throw new InvalidOperationException("This function is only for ongoing schedules.");

            var agencyId  = schedule.Itinerary?.AgencyId
                ?? throw new InvalidOperationException("Cannot determine agency for this schedule");
            var itinerary = schedule.Itinerary!;
            var tourName  = itinerary.Name ?? scheduleId.ToString();
            var now       = DateTime.UtcNow;
            var result    = new AgencyCancelResultDTO { ScheduleId = scheduleId };

            // 1. Active bookings and pax
            var biLines     = await _biRepo.GetByScheduleIdAsync(scheduleId);
            var activeLines = biLines.Where(l => l.Status != BookingItineraryStatus.Cancelled).ToList();
            int actualPax   = activeLines.Sum(l => l.NumberOfGuests);

            // 2. Completion ratio
            var stops         = await _stopRepo.GetWithProviderAndActivitiesByItineraryIdAsync(schedule.ItineraryId);
            var allActivities = stops.SelectMany(s => s.Activities).ToList();
            long totalActivitiesPrice     = allActivities.Sum(a => a.Price);
            long completedActivitiesPrice = 0;

            foreach (var act in allActivities)
            {
                int dayOffset  = act.StartTime.Year == 2000 ? (int)(act.StartTime.Date - ActivityBaseDate.Date).TotalDays : 0;
                var actEndTime = schedule.StartTime.Date.AddDays(dayOffset).Add(act.EndTime.TimeOfDay);
                if (actEndTime < now)
                    completedActivitiesPrice += act.Price;
            }

            double completionRatio = totalActivitiesPrice > 0
                ? (double)completedActivitiesPrice / totalActivitiesPrice
                : Math.Clamp((now - schedule.StartTime).TotalSeconds / (schedule.EndTime - schedule.StartTime).TotalSeconds, 0, 1);

            result.CompletionRatioPercent = Math.Round(completionRatio * 100, 2);

            // 3. Process deposits
            //    - service already done       → return deposit to agency  (DepositStatus.Returned)
            //    - upcoming service, >48h     → refund deposit to agency  (DepositStatus.Refunded)
            //    - upcoming service, ≤48h     → provider keeps deposit    (DepositStatus.Forfeited)
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var toUpdate = new List<ProviderDeposit>();

            foreach (var deposit in deposits.Where(d => d.Status == DepositStatus.Paid))
            {
                if (deposit.ActualFirstActivityTime <= now)
                {
                    deposit.Status = DepositStatus.Returned;
                    toUpdate.Add(deposit);
                    await CreditAgencyWalletAsync(agencyId, deposit.DepositAmount, scheduleId,
                        $"Hoàn cọc {deposit.ProviderName} (dịch vụ đã hoàn thành, tour bị hủy)");
                    result.TotalRefunded += deposit.DepositAmount;
                    result.Details.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Returned",
                        Reason                  = "Service already completed",
                    });
                }
                else if (now < deposit.ActualFirstActivityTime.AddHours(-CancelWindowHours))
                {
                    deposit.Status = DepositStatus.Refunded;
                    toUpdate.Add(deposit);
                    await CreditAgencyWalletAsync(agencyId, deposit.DepositAmount, scheduleId,
                        $"Hoàn cọc {deposit.ProviderName} (hủy ongoing > {CancelWindowHours}h trước dịch vụ)");
                    result.TotalRefunded += deposit.DepositAmount;
                    result.Details.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Refunded",
                        Reason                  = $"Upcoming service, more than {CancelWindowHours}h away",
                    });
                }
                else
                {
                    deposit.Status = DepositStatus.Forfeited;
                    toUpdate.Add(deposit);
                    await CreditProviderWalletAsync(deposit.ProviderId, deposit.DepositAmount, scheduleId,
                        $"Giữ cọc (agency hủy tour đang diễn ra trong vòng {CancelWindowHours}h dịch vụ)");
                    result.TotalForfeited += deposit.DepositAmount;
                    result.Details.Add(new DepositCancelDetail
                    {
                        ProviderName            = deposit.ProviderName,
                        DepositAmount           = deposit.DepositAmount,
                        ActualFirstActivityTime = deposit.ActualFirstActivityTime,
                        Outcome                 = "Forfeited",
                        Reason                  = $"Upcoming service within {CancelWindowHours}h — provider keeps deposit",
                    });
                }
            }

            if (toUpdate.Any())
                await _depositRepo.UpdateRangeAsync(toUpdate);

            // 4. Credit agency for completed services value
            //    = tour price × completion ratio × actual pax
            long agencyServiceEarnings = (long)(itinerary.Price * completionRatio * actualPax);
            if (agencyServiceEarnings > 0)
            {
                await CreditAgencyWalletAsync(agencyId, agencyServiceEarnings, scheduleId,
                    $"Doanh thu dịch vụ đã hoàn thành ({completionRatio:P0}) — \"{tourName}\" bị hủy giữa chừng");
                result.AgencyServiceEarnings = agencyServiceEarnings;
            }

            // 5. Refund customers: amount paid − used services value
            var processedBookingIds = new HashSet<Guid>();
            foreach (var line in activeLines)
            {
                var booking = await _bookingRepo.GetByIdAsync(line.BookingId);
                if (booking == null) continue;

                if (processedBookingIds.Add(booking.Id))
                {
                    var payment = await _paymentRepo.GetPaidPaymentByBookingIdAsync(booking.Id);
                    if (payment != null && payment.FinalAmount > 0)
                    {
                        long usedValue    = (long)(payment.FinalAmount * completionRatio);
                        long refundAmount = payment.FinalAmount - usedValue;

                        if (refundAmount > 0)
                        {
                            await CreditCustomerWalletAsync(
                                booking.UserId,
                                refundAmount,
                                booking.Id,
                                $"Hoàn tiền booking #{booking.Code} (tour bị hủy giữa chừng, đã dùng {completionRatio:P0})");

                            payment.Status    = PaymentStatus.Refunded;
                            payment.UpdatedAt = DateTime.UtcNow;
                            await _paymentRepo.UpdateAsync(payment);

                            await _refundRepo.CreateAsync(new Refund
                            {
                                Id                = Guid.NewGuid(),
                                BookingId         = booking.Id,
                                PaymentId         = payment.Id,
                                TotalRefundAmount = refundAmount,
                                InitiatedBy       = RefundInitinator.Agency,
                                Reason            = "Tour ongoing cancelled by agency",
                                Status            = RefundStatus.Completed,
                                RefundedAt        = DateTime.UtcNow,
                                AdminNote         = $"Hoàn {refundAmount:N0}đ ({(1 - completionRatio):P0} chưa sử dụng)",
                                CreatedAt         = DateTime.UtcNow,
                                UpdatedAt         = DateTime.UtcNow,
                            });

                            await _notificationRepo.CreateAsync(new Notification
                            {
                                Id              = Guid.NewGuid(),
                                RecipientUserId = booking.UserId,
                                Title           = "Tour cancelled — partial refund processed",
                                Message         = $"Tour \"{tourName}\" was cancelled by the agency. " +
                                                  $"Booking #{booking.Code}: {refundAmount:N0}đ has been credited to your wallet.",
                                EntityType      = NotificationEntityType.Booking,
                                EntityId        = booking.Id,
                                IsRead          = false,
                                CreatedAt       = DateTime.UtcNow,
                            });

                            result.CustomersRefunded++;
                            result.TotalCustomerRefunded += refundAmount;
                        }

                        booking.Status    = BookingStatus.Cancelled;
                        booking.UpdatedAt = DateTime.UtcNow;
                        await _bookingRepo.UpdateAsync(booking);

                        result.BookingsCancelled++;
                    }
                }

                line.Status    = BookingItineraryStatus.Cancelled;
                line.UpdatedAt = DateTime.UtcNow;
                await _biRepo.UpdateAsync(line);
            }

            // 6. Finalize schedule
            schedule.Status    = ItineraryScheduleStatus.Cancelled;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _scheduleRepo.UpdateAsync(schedule);

            await NotifyAgencyOnScheduleCancelledAsync(schedule, agencyId, scheduleId, tourName, result, cancelledByAdmin: false);
            await NotifyProvidersOnScheduleCancelledAsync(schedule.ItineraryId, scheduleId, tourName, cancelledByAdmin: false);

            return result;
        }

        /// <summary>
        /// Called when a schedule is marked Completed.
        /// Returns all Paid deposits back to the agency wallet (they served their purpose).
        /// </summary>
        public async Task ReturnDepositsOnCompletedAsync(Guid scheduleId, Guid agencyId)
        {
            var deposits = await _depositRepo.GetByScheduleIdAsync(scheduleId);
            var paid     = deposits.Where(d => d.Status == DepositStatus.Paid).ToList();
            if (!paid.Any()) return;

            var total = paid.Sum(d => d.DepositAmount);

            foreach (var d in paid) d.Status = DepositStatus.Returned;
            await _depositRepo.UpdateRangeAsync(paid);

            await CreditAgencyWalletAsync(agencyId, total, scheduleId, "Hoàn tiền cọc provider sau khi tour hoàn thành");
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task<ItinerarySchedule> GetCancellableScheduleAsync(Guid scheduleId)
        {
            var schedule = await _scheduleRepo.GetScheduleWithDetails(scheduleId)
                ?? throw new KeyNotFoundException("Schedule not found");

            if (schedule.Status == ItineraryScheduleStatus.Cancelled)
                throw new InvalidOperationException("Schedule is already cancelled");

            if (schedule.Status == ItineraryScheduleStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed schedule");

            return schedule;
        }

        private async Task<(int affectedBookings, long totalCustomerRefund)> CalculateCustomerRefundPreviewAsync(Guid scheduleId)
        {
            var lines = await _biRepo.GetByScheduleIdAsync(scheduleId);
            var activeLines = lines.Where(l => l.Status != BookingItineraryStatus.Cancelled).ToList();
            var bookingIds = activeLines.Select(l => l.BookingId).Distinct().ToList();

            var affectedBookings = 0;
            long totalCustomerRefund = 0;

            foreach (var bookingId in bookingIds)
            {
                var booking = await _bookingRepo.GetByIdAsync(bookingId);
                if (booking == null || booking.Status == BookingStatus.Cancelled)
                    continue;

                affectedBookings++;
                var payment = await _paymentRepo.GetPaidPaymentByBookingIdAsync(bookingId);
                if (payment == null || payment.FinalAmount <= 0)
                    continue;

                var existingRefund = await _refundRepo.GetByBookingIdAsync(bookingId);
                if (existingRefund?.Status == RefundStatus.Completed)
                    continue;

                totalCustomerRefund += payment.FinalAmount;
            }

            return (affectedBookings, totalCustomerRefund);
        }

        private async Task CancelBookingsAndScheduleAsync(
            ItinerarySchedule schedule,
            Guid scheduleId,
            string tourName,
            AgencyCancelResultDTO result,
            bool cancelledByAdmin)
        {
            var lines = await _biRepo.GetByScheduleIdAsync(scheduleId);
            var activeLines = lines.Where(l => l.Status != BookingItineraryStatus.Cancelled).ToList();
            var refundedBookingIds = new HashSet<Guid>();
            var cancelledBy = cancelledByAdmin ? "admin" : "agency";

            foreach (var line in activeLines)
            {
                var booking = await _bookingRepo.GetByIdAsync(line.BookingId);
                if (booking != null && booking.Status != BookingStatus.Cancelled)
                {
                    long? refundAmount = null;
                    if (refundedBookingIds.Add(booking.Id))
                        refundAmount = await RefundCustomerBookingAsync(booking, result, cancelledByAdmin);

                    booking.Status    = BookingStatus.Cancelled;
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingRepo.UpdateAsync(booking);

                    await _notificationRepo.CreateAsync(new Notification
                    {
                        Id              = Guid.NewGuid(),
                        RecipientUserId = booking.UserId,
                        Title           = refundAmount > 0 ? "Refund received — schedule cancelled" : "Tour schedule cancelled",
                        Message         = refundAmount > 0
                            ? $"Tour \"{tourName}\" was cancelled by the {cancelledBy}. Booking #{booking.Code}: {refundAmount:N0}đ has been credited to your wallet."
                            : $"Tour \"{tourName}\" was cancelled by the {cancelledBy}. Booking #{booking.Code} has been cancelled.",
                        EntityType      = NotificationEntityType.Booking,
                        EntityId        = booking.Id,
                        IsRead          = false,
                        CreatedAt       = DateTime.UtcNow,
                    });

                    result.BookingsCancelled++;
                }

                line.Status    = BookingItineraryStatus.Cancelled;
                line.UpdatedAt = DateTime.UtcNow;
                await _biRepo.UpdateAsync(line);
            }

            schedule.SpotLeft += activeLines.Sum(l => l.NumberOfGuests);
            schedule.Status    = ItineraryScheduleStatus.Cancelled;
            schedule.UpdatedAt = DateTime.UtcNow;
            await _scheduleRepo.UpdateAsync(schedule);
        }

        /// <summary>
        /// Refund held booking payment to customer when a schedule is cancelled.
        /// Agency was never credited on payment — only customer wallet is restored.
        /// </summary>
        private async Task<long?> RefundCustomerBookingAsync(
            Booking booking, AgencyCancelResultDTO result, bool cancelledByAdmin)
        {
            var payment = await _paymentRepo.GetPaidPaymentByBookingIdAsync(booking.Id);
            if (payment == null || payment.FinalAmount <= 0)
                return null;

            var existingRefund = await _refundRepo.GetByBookingIdAsync(booking.Id);
            if (existingRefund?.Status == RefundStatus.Completed)
                return null;

            var refundAmount = payment.FinalAmount;
            var reason = cancelledByAdmin ? "Schedule cancelled by admin" : "Schedule cancelled by agency";

            await CreditCustomerWalletAsync(
                booking.UserId,
                refundAmount,
                booking.Id,
                $"Refund for booking #{booking.Code} ({reason.ToLower()})");

            payment.Status    = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentRepo.UpdateAsync(payment);

            if (existingRefund == null)
            {
                await _refundRepo.CreateAsync(new Refund
                {
                    Id                = Guid.NewGuid(),
                    BookingId         = booking.Id,
                    PaymentId         = payment.Id,
                    TotalRefundAmount = refundAmount,
                    InitiatedBy       = RefundInitinator.Admin,
                    Reason            = reason,
                    Status            = RefundStatus.Completed,
                    RefundedAt        = DateTime.UtcNow,
                    AdminNote         = cancelledByAdmin
                        ? "Auto-refunded to customer wallet on admin schedule cancellation"
                        : "Auto-refunded to customer wallet on agency schedule cancellation",
                    CreatedAt         = DateTime.UtcNow,
                    UpdatedAt         = DateTime.UtcNow,
                });
            }

            result.CustomersRefunded++;
            result.TotalCustomerRefunded += refundAmount;
            return refundAmount;
        }

        private async Task NotifyAgencyOnScheduleCancelledAsync(
            ItinerarySchedule schedule, Guid agencyId, Guid scheduleId, string tourName, AgencyCancelResultDTO result,
            bool cancelledByAdmin)
        {
            var notifiedUserIds = new HashSet<Guid>();

            string managerTitle, managerMsg;
            if (cancelledByAdmin)
            {
                managerTitle = "Schedule cancelled by admin";
                managerMsg   = $"Schedule \"{tourName}\" was cancelled by admin. " +
                               $"{result.TotalRefunded:N0}đ deposit has been fully credited to your wallet.";
                if (result.TotalCustomerRefunded > 0)
                    managerMsg += $" {result.CustomersRefunded} customer booking(s) refunded ({result.TotalCustomerRefunded:N0}đ).";
            }
            else if (result.TotalRefunded > 0 && result.TotalForfeited > 0)
            {
                managerTitle = "Deposit refunded to wallet — schedule cancelled";
                managerMsg   = $"Schedule \"{tourName}\" cancelled. {result.TotalRefunded:N0}đ deposit has been credited to your wallet. " +
                               $"{result.TotalForfeited:N0}đ forfeited (cancelled within {CancelWindowHours}h of a provider service).";
            }
            else if (result.TotalRefunded > 0)
            {
                managerTitle = "Deposit refunded to wallet — schedule cancelled";
                managerMsg   = $"Schedule \"{tourName}\" cancelled. {result.TotalRefunded:N0}đ deposit has been credited to your wallet.";
            }
            else if (result.TotalForfeited > 0)
            {
                managerTitle = "Schedule cancelled — deposit forfeited";
                managerMsg   = $"Schedule \"{tourName}\" cancelled. {result.TotalForfeited:N0}đ deposit forfeited (cancelled within {CancelWindowHours}h of a provider service).";
            }
            else
            {
                managerTitle = "Schedule cancelled";
                managerMsg   = $"Schedule \"{tourName}\" has been cancelled.";
            }

            var agencyUsers = await _agencyUserRepo.GetAgencyUsers(agencyId);
            foreach (var manager in agencyUsers.Where(u => u.Role == AgencyUserRole.Manager))
            {
                if (!notifiedUserIds.Add(manager.UserId))
                    continue;

                await _notificationRepo.CreateAsync(new Notification
                {
                    Id              = Guid.NewGuid(),
                    RecipientUserId = manager.UserId,
                    Title           = managerTitle,
                    Message         = managerMsg,
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = scheduleId,
                    IsRead          = false,
                    CreatedAt       = DateTime.UtcNow,
                });
            }

            if (schedule.GuideId.HasValue && notifiedUserIds.Add(schedule.GuideId.Value))
            {
                await _notificationRepo.CreateAsync(new Notification
                {
                    Id              = Guid.NewGuid(),
                    RecipientUserId = schedule.GuideId.Value,
                    Title           = "Schedule cancelled",
                    Message         = $"The schedule for tour \"{tourName}\" has been cancelled. Your tour guide assignment on this schedule is no longer active.",
                    EntityType      = NotificationEntityType.Itinerary,
                    EntityId        = scheduleId,
                    IsRead          = false,
                    CreatedAt       = DateTime.UtcNow,
                });
            }
        }

        private async Task NotifyProvidersOnScheduleCancelledAsync(
            Guid itineraryId, Guid scheduleId, string tourName, bool cancelledByAdmin)
        {
            var stops = await _stopRepo.GetWithProviderAndActivitiesByItineraryIdAsync(itineraryId);
            var providerIds = stops
                .Where(s => s.ProviderId.HasValue)
                .Select(s => s.ProviderId!.Value)
                .Distinct()
                .ToList();

            var notifiedUserIds = new HashSet<Guid>();
            var actor = cancelledByAdmin ? "admin" : "agency";

            foreach (var providerId in providerIds)
            {
                var providerName = stops.First(s => s.ProviderId == providerId).Provider?.Name ?? "Provider";
                var managers = await _providerUserRepo.GetManagersByProviderIdAsync(providerId);

                foreach (var manager in managers)
                {
                    if (!notifiedUserIds.Add(manager.UserId))
                        continue;

                    await _notificationRepo.CreateAsync(new Notification
                    {
                        Id              = Guid.NewGuid(),
                        RecipientUserId = manager.UserId,
                        Title           = "Tour schedule cancelled",
                        Message         = $"The {actor} has cancelled the schedule for tour \"{tourName}\". " +
                                          $"Services for {providerName} on this trip will not proceed.",
                        EntityType      = NotificationEntityType.Itinerary,
                        EntityId        = scheduleId,
                        IsRead          = false,
                        CreatedAt       = DateTime.UtcNow,
                    });
                }
            }
        }

        private async Task CreditCustomerWalletAsync(Guid userId, long amount, Guid referenceId, string note)
        {
            var wallet = await _walletRepo.GetByUserIdAsync(userId)
                ?? await _walletRepo.CreateAsync(new Wallet
                {
                    Id             = Guid.NewGuid(),
                    UserId         = userId,
                    Balance        = 0,
                    PendingBalance = 0,
                    CreatedAt      = DateTime.UtcNow,
                });

            wallet.Balance  += amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet);

            await _walletTxRepo.CreateAsync(new WalletTransaction
            {
                Id          = Guid.NewGuid(),
                WalletId    = wallet.Id,
                Amount      = amount,
                Type        = WalletTransactionType.Credit,
                Reason      = WalletTransactionReason.Refund,
                ReferenceId = referenceId,
                Note        = note,
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow,
            });
        }

        private async Task CreditAgencyWalletAsync(Guid agencyId, long amount, Guid referenceId, string note)
        {
            var agencyManager = await _agencyUserRepo.GetManagerByAgencyIdAsync(agencyId)
                ?? throw new InvalidOperationException("Agency manager not found");

            var wallet = await _walletRepo.GetByUserIdAsync(agencyManager.UserId)
                ?? await _walletRepo.CreateAsync(new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = agencyManager.UserId,
                    Balance = 0,
                    PendingBalance = 0,
                    CreatedAt = DateTime.UtcNow,
                });

            wallet.Balance   += amount;
            wallet.UpdatedAt  = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet);

            await _walletTxRepo.CreateAsync(new WalletTransaction
            {
                Id          = Guid.NewGuid(),
                WalletId    = wallet.Id,
                Amount      = amount,
                Type        = WalletTransactionType.Credit,
                Reason      = WalletTransactionReason.Refund,
                ReferenceId = referenceId,
                Note        = note,
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow,
            });
        }

        private async Task CreditProviderWalletAsync(Guid providerId, long amount, Guid referenceId, string note)
        {
            var managers = await _providerUserRepo.GetManagersByProviderIdAsync(providerId);
            var manager = managers.FirstOrDefault() ?? throw new InvalidOperationException("Provider manager not found");

            var wallet = await _walletRepo.GetByUserIdAsync(manager.UserId)
                ?? await _walletRepo.CreateAsync(new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = manager.UserId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                });

            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(wallet);

            await _walletTxRepo.CreateAsync(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                Amount = amount,
                Type = WalletTransactionType.Credit,
                Reason = WalletTransactionReason.BookingEarning,
                ReferenceId = referenceId,
                Note = note,
                CreatedAt = DateTime.UtcNow,
            });
        }

        private static List<DepositCancelDetail> BuildDetails(List<ProviderDeposit> deposits, DateTime now)
            => deposits.Select(d =>
            {
                string outcome, reason;
                if (d.Status != DepositStatus.Paid)
                {
                    outcome = d.Status.ToString();
                    reason  = "Already processed";
                }
                else if (now < d.ActualFirstActivityTime.AddHours(-CancelWindowHours))
                {
                    outcome = "Refunded";
                    reason  = $"More than {CancelWindowHours}h before service";
                }
                else
                {
                    outcome = "Forfeited";
                    reason  = $"Within {CancelWindowHours}h of service";
                }
                return new DepositCancelDetail
                {
                    ProviderName            = d.ProviderName,
                    DepositAmount           = d.DepositAmount,
                    ActualFirstActivityTime = d.ActualFirstActivityTime,
                    Outcome                 = outcome,
                    Reason                  = reason,
                };
            }).ToList();

        private static ProviderDepositDTO MapDTO(ProviderDeposit d) => new()
        {
            Id                      = d.Id,
            ScheduleId              = d.ItineraryScheduleId,
            ProviderId              = d.ProviderId,
            ProviderName            = d.ProviderName,
            ActualFirstActivityTime = d.ActualFirstActivityTime,
            ServiceTotal            = d.ServiceTotal,
            DepositAmount           = d.DepositAmount,
            Status                  = d.Status.ToString(),
        };
    }
}

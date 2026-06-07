using TouRest.Application.DTOs.Deposit;

namespace TouRest.Application.Interfaces
{
    public interface IDepositService
    {
        /// <summary>Calculate deposit breakdown for an itinerary before creating a schedule.</summary>
        Task<DepositCalculationDTO> CalculateAsync(Guid itineraryId, DateTime scheduleStartTime);

        /// <summary>Get deposit summary for an existing schedule.</summary>
        Task<BookingDepositSummaryDTO> GetByScheduleAsync(Guid scheduleId);

        /// <summary>Preview refund/forfeit outcome if agency cancels a Pending/Confirmed schedule NOW.</summary>
        Task<ScheduleCancelPreviewDTO> PreviewCancelAsync(Guid scheduleId);

        /// <summary>
        /// Agency cancels a Pending/Confirmed schedule.
        /// Per provider: &gt;48h before service → refund to agency wallet; ≤48h → forfeit.
        /// Also cancels all bookings.
        /// </summary>
        Task<AgencyCancelResultDTO> AgencyCancelScheduleAsync(Guid scheduleId);

        /// <summary>Preview refund/earning outcome if agency cancels an ONGOING schedule NOW.</summary>
        Task<OngoingCancelPreviewDTO> PreviewOngoingCancelAsync(Guid scheduleId);

        /// <summary>
        /// Agency cancels an ONGOING schedule.
        /// Agency receives: tour price × completion ratio × pax + deposits returned/refunded.
        /// Provider receives: deposit for upcoming services ≤48h (cancellation penalty).
        /// Customer receives: amount paid − used services value.
        /// </summary>
        Task<AgencyCancelResultDTO> AgencyCancelOngoingScheduleAsync(Guid scheduleId);

        /// <summary>Preview full refund outcome if admin cancels this schedule NOW.</summary>
        Task<AdminScheduleCancelPreviewDTO> PreviewAdminCancelAsync(Guid scheduleId);

        /// <summary>
        /// Admin cancels a schedule.
        /// Refunds ALL provider deposits to agency (no 48h rule) and full trip cost to customers.
        /// </summary>
        Task<AgencyCancelResultDTO> AdminCancelScheduleAsync(Guid scheduleId);

        /// <summary>Called when schedule is marked Completed — returns all deposits to agency wallet.</summary>
        Task ReturnDepositsOnCompletedAsync(Guid scheduleId, Guid agencyId);
    }
}

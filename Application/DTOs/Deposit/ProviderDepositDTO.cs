namespace TouRest.Application.DTOs.Deposit
{
    public class ProviderDepositDTO
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = "";
        public DateTime ActualFirstActivityTime { get; set; }
        public long ServiceTotal { get; set; }
        public long DepositAmount { get; set; }
        public string Status { get; set; } = "";
    }

    /// <summary>Deposit summary for a schedule (returned after schedule creation).</summary>
    public class BookingDepositSummaryDTO
    {
        public Guid ScheduleId { get; set; }
        public long TotalDepositAmount { get; set; }
        public List<ProviderDepositDTO> ProviderDeposits { get; set; } = [];
    }

    /// <summary>
    /// Preview of deposit amounts before agency creates a schedule.
    /// Returned by GET /api/deposits/calculate?itineraryId=...
    /// </summary>
    public class DepositCalculationDTO
    {
        public Guid ItineraryId { get; set; }
        public long TotalDepositAmount { get; set; }
        public List<ProviderDepositDTO> Providers { get; set; } = [];
    }

    /// <summary>Preview of refund/forfeit outcome if agency cancels NOW.</summary>
    public class ScheduleCancelPreviewDTO
    {
        public Guid ScheduleId { get; set; }
        public int AffectedBookings { get; set; }
        public long TotalToRefund { get; set; }
        public long TotalToForfeit { get; set; }
        public List<DepositCancelDetail> Details { get; set; } = [];
    }

    /// <summary>Preview when admin cancels — full deposit + full customer refund.</summary>
    public class AdminScheduleCancelPreviewDTO
    {
        public Guid ScheduleId { get; set; }
        public int AffectedBookings { get; set; }
        public long TotalDepositRefund { get; set; }
        public long TotalCustomerRefund { get; set; }
    }

    public class DepositCancelDetail
    {
        public string ProviderName { get; set; } = "";
        public long DepositAmount { get; set; }
        public DateTime ActualFirstActivityTime { get; set; }
        public string Outcome { get; set; } = ""; // Refunded | Forfeited
        public string Reason { get; set; } = "";
    }

    public class AgencyCancelResultDTO
    {
        public Guid ScheduleId { get; set; }
        public int BookingsCancelled { get; set; }
        public int CustomersRefunded { get; set; }
        /// <summary>Total deposits returned/refunded to agency wallet.</summary>
        public long TotalRefunded { get; set; }
        public long TotalCustomerRefunded { get; set; }
        /// <summary>Total deposits kept by providers (≤48h upcoming services).</summary>
        public long TotalForfeited { get; set; }
        /// <summary>Revenue credited to agency for completed services (ongoing cancel only).</summary>
        public long AgencyServiceEarnings { get; set; }
        /// <summary>Percentage of tour completed at time of cancel (ongoing cancel only).</summary>
        public double CompletionRatioPercent { get; set; }
        public List<DepositCancelDetail> Details { get; set; } = [];
    }

    /// <summary>Preview of what will happen if agency cancels an ONGOING schedule now.</summary>
    public class OngoingCancelPreviewDTO
    {
        public Guid ScheduleId { get; set; }
        public double CompletionRatioPercent { get; set; }
        public int AffectedBookings { get; set; }
        public int ActualPassengers { get; set; }
        /// <summary>tour price × completion ratio × pax — credited to agency.</summary>
        public long AgencyServiceEarnings { get; set; }
        /// <summary>Deposits for completed services + upcoming >48h — returned to agency.</summary>
        public long DepositsReturnedToAgency { get; set; }
        /// <summary>Deposits for upcoming ≤48h services — kept by providers.</summary>
        public long DepositsKeptByProviders { get; set; }
        public long TotalCustomerRefund { get; set; }
        public List<DepositCancelDetail> DepositDetails { get; set; } = [];
    }
}

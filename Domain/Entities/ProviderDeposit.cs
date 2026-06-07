using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TouRest.Domain.Base;
using TouRest.Domain.Enums;

namespace TouRest.Domain.Entities
{
    /// <summary>
    /// Tracks the 20% deposit the AGENCY pays per provider stop when creating a schedule.
    /// Deposit = sum(ItineraryActivity.Price for this stop) × 20%.
    ///
    /// Status transitions:
    ///   Paid       (deducted from agency wallet on schedule creation)
    ///   → Refunded (agency cancelled > 48h before service → money back to agency wallet)
    ///   → Forfeited (agency cancelled ≤ 48h → admin keeps as provider compensation)
    ///   → Returned  (schedule completed normally → deposit returned to agency wallet)
    /// </summary>
    [Table("provider_deposits")]
    public class ProviderDeposit : BaseEntity
    {
        [Required]
        public Guid ItineraryScheduleId { get; set; }

        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProviderName { get; set; } = "";

        [Required]
        public Guid ItineraryStopId { get; set; }

        /// <summary>
        /// Pre-computed real datetime of this provider's first activity in this schedule run.
        /// Used for the 48-hour cancellation rule.
        /// </summary>
        [Required]
        public DateTime ActualFirstActivityTime { get; set; }

        /// <summary>Sum of all ItineraryActivity.Price for this provider stop.</summary>
        [Range(0, long.MaxValue)]
        public long ServiceTotal { get; set; }

        /// <summary>ServiceTotal × 20% — deducted from agency wallet on schedule creation.</summary>
        [Range(0, long.MaxValue)]
        public long DepositAmount { get; set; }

        [Required]
        public DepositStatus Status { get; set; } = DepositStatus.Paid;

        // Navigation
        public ItinerarySchedule ItinerarySchedule { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public ItineraryStop ItineraryStop { get; set; } = null!;
    }
}

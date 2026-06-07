namespace TouRest.Domain.Enums
{
    public enum DepositStatus
    {
        Paid,      // agency paid when creating schedule (default)
        Refunded,  // agency cancelled > 48h before service → returned to agency wallet
        Forfeited, // agency cancelled ≤ 48h → admin keeps as compensation
        Returned   // schedule completed normally → deposit returned to agency wallet
    }
}

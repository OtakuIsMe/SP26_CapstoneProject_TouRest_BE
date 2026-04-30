namespace TouRest.Application.DTOs.Booking
{
    public class BookingCreateResponse
    {
        public Guid BookingId { get; set; }
        public string Code { get; set; } = "";
        public int TotalAmount { get; set; }
        public int BaseAmount { get; set; }
        public int DiscountAmount { get; set; }
        public string? VoucherApplied { get; set; }
    }
}

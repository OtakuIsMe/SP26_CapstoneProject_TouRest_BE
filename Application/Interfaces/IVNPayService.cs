using TouRest.Application.DTOs.Payment;

namespace TouRest.Application.Interfaces
{
    public interface IVNPayService
    {
        Task<VNPayQrResponse> GenerateQrAsync(Guid bookingId, string ipAddr, int expireMinutes = 15);
    }
}

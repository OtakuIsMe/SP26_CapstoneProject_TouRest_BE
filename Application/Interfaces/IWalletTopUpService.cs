using TouRest.Application.DTOs.Wallet;

namespace TouRest.Application.Interfaces
{
    public interface IWalletTopUpService
    {
        Task<WalletTopUpDTO> CreateTopUpAsync(Guid userId, long amount);
        Task<WalletTopUpDTO> GetStatusAsync(long orderCode);
        Task FinalizeTopUpByOrderCodeAsync(long orderCode, Guid userId);
    }
}

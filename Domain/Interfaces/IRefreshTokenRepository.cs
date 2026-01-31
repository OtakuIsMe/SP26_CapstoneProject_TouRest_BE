using TouRest.Domain.Entities;

namespace TouRest.Domain.Interfaces
{
    public interface IRefreshTokenRepository: IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(string token);
        Task RevokeAllByUserAsync(Guid userId);
    }
}
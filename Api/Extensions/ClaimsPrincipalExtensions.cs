using System.Security.Claims;
using TouRest.Domain.Enums;

namespace TouRest.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user token");

            var statusClaim = user.FindFirst("status");
            if (statusClaim == null || statusClaim.Value != nameof(UserStatus.Active))
                throw new UnauthorizedAccessException("User account is not active");

            return userId;
        }
    }
}
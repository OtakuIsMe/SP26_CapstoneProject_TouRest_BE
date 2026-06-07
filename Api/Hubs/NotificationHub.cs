using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TouRest.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}

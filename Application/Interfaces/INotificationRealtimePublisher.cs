using TouRest.Domain.Entities;

namespace TouRest.Application.Interfaces
{
    public interface INotificationRealtimePublisher
    {
        Task PublishAsync(Notification notification);
    }
}

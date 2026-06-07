using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using TouRest.Api.Hubs;
using TouRest.Application.DTOs.Notification;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;

namespace TouRest.Api.Services
{
    public class SignalRNotificationPublisher : INotificationRealtimePublisher
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task PublishAsync(Notification notification)
        {
            var dto = _mapper.Map<NotificationDTO>(notification);
            await _hubContext.Clients
                .User(notification.RecipientUserId.ToString())
                .SendAsync("ReceiveNotification", dto);
        }
    }
}

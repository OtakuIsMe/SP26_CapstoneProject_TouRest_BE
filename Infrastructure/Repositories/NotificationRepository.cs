using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;

namespace TouRest.Infrastructure.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private readonly INotificationRealtimePublisher _publisher;

        public NotificationRepository(
            AppDbContext context,
            INotificationRealtimePublisher publisher) : base(context)
        {
            _publisher = publisher;
        }

        public override async Task<Notification> CreateAsync(Notification entity)
        {
            var created = await base.CreateAsync(entity);
            await _publisher.PublishAsync(created);
            return created;
        }
        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientUserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications.CountAsync(n => n.RecipientUserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await _context.Notifications.Where(n => n.Id == notificationId)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _context.Notifications.Where(n => n.RecipientUserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}

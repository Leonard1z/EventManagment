using Infrastructure.Repositories.Notifications;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Notification
{
    public class NotificationService:INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Domain.Entities.Notification> Create(Domain.Entities.Notification notification)
        {
            return await _notificationRepository.CreateAsync(notification);
        }

        public async Task<int> GetNotificationCountByUserId(int userId)
        {
            var notificationCount = await _notificationRepository.GetNotificationCountByUserId(userId);

            return notificationCount;
        }
    }
}

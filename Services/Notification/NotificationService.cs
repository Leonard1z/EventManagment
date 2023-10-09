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

        public async Task<IList<Domain.Entities.Notification>> GetNotificationCountAndDataByUserId(int userId)
        {
            return await _notificationRepository.GetNotificationCountAndDataByUserId(userId);
        }
    }
}

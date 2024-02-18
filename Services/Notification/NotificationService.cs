using Infrastructure.Repositories.Notifications;

namespace Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Domain.Entities.Notification> Create(Domain.Entities.Notification notification)
        {
            return _notificationRepository.Create(notification);
            
        }

        public async Task<IList<Domain.Entities.Notification>> GetAdminNotifications()
        {
            return await _notificationRepository.GetAdminNotifications();
        }
        public async Task<int> GetAdminUnreadNotificationCount()
        {
            return await _notificationRepository.GetAdminUnreadNotificationCount();
        }
        public async Task<Domain.Entities.Notification> GetById(int id)
        {
            var notification = await _notificationRepository.GetById(id);

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);

            return notification;
        }

        public async Task<IList<Domain.Entities.Notification>> GetNotificationDataByUserId(int userId)
        {
            return await _notificationRepository.GetNotificationDataByUserId(userId);
        }

        public async Task<int> GetUnreadNotificationCountByUserId(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationCountByUserId(userId);
        }

        public async Task MarkAllNotificationAsRead()
        {
            var notifications = await _notificationRepository.GetAllAdminNotifications(); 

            foreach(var notification in notifications)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
        }
    }
}

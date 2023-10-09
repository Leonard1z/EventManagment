using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Services.Notification;

namespace EventManagment.Hubs
{
    public class NotificationHub :Hub
    {
        private readonly INotificationService _notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var userId = Context.UserIdentifier;
            var modifiedNotification = await _notificationService.GetById(notificationId);
            await Clients.User(userId).SendAsync("MarkNotificationAsRead", modifiedNotification);
        }
    }
    
}

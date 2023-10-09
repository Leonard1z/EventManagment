using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace EventManagment.Hubs
{
    public class NotificationHub :Hub
    {
        public async Task UpdateNotificationCountAndData(int notificationCount, List<Notification> notificationsData)
        {
            var userId = Context.UserIdentifier;
            await Clients.User(userId).SendAsync("UpdateNotificationCountAndData", notificationCount, notificationsData);
        }
    }
    
}

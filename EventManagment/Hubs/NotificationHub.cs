using Microsoft.AspNetCore.SignalR;

namespace EventManagment.Hubs
{
    public class NotificationHub :Hub
    {
        public async Task UpdateNotificationCount(int count)
        {
            var userId = Context.UserIdentifier;
            await Clients.User(userId).SendAsync("UpdateNotificationCount", count);
        }
    }
    
}

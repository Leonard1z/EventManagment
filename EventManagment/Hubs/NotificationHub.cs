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
        public async Task MarkAllNotificationsAsRead()
        {
            await _notificationService.MarkAllNotificationAsRead();
            await Clients.Caller.SendAsync("MarkAllNotificationsAsRead");
        }
        public async Task AddAdminConnection()
        {
            if (Context.User.IsInRole("Admin"))
            {
                var adminConnectionId = Context.ConnectionId;
         
                await Groups.AddToGroupAsync(adminConnectionId, "Admins");
                
            }
        }
        public async Task RemoveAdminConnection()
        {
            if (Context.User.IsInRole("Admin"))
            {
                var adminConnectionId = Context.ConnectionId;

                await Groups.RemoveFromGroupAsync(adminConnectionId, "Admins");
            }

        }
        public override async Task OnConnectedAsync()
        {
            await AddAdminConnection();
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveAdminConnection();
            await base.OnDisconnectedAsync(exception);
        }
    }
    
}

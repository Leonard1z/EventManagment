using Services.Common;

namespace Services.Notification
{
    public interface INotificationService : IService
    {
        Task<Domain.Entities.Notification> Create(Domain.Entities.Notification notification);
        Task<IList<Domain.Entities.Notification>> GetAdminNotifications();
        Task<int> GetAdminUnreadNotificationCount();
        Task<IList<Domain.Entities.Notification>> GetNotificationDataByUserId(int userId);
        Task<Domain.Entities.Notification> GetById(int id);
        Task<int> GetUnreadNotificationCountByUserId(int userId);
        Task MarkAllNotificationAsRead();
    }
}

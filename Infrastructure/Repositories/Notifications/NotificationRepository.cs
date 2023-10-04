using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Notifications
{
    public class NotificationRepository:GenericRepository<Notification>,INotificationRepository
    {
        public NotificationRepository(EventManagmentDb context) : base(context)
        {

        }

        public async Task<int> GetNotificationCountByUserId(int userId)
        {
            var count = DbSet.Count(n => n.UserId == userId);

            return count;
        }
    }
}

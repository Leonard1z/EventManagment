using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IList<Notification>> GetNotificationCountAndDataByUserId(int userId)
        {
            return await DbSet.Where(n => n.UserId == userId).Select(n=> new Notification
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
            }).ToListAsync();
        }
    }
}

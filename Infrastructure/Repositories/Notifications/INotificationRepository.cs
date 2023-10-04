using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Notifications
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<int> GetNotificationCountByUserId(int userId);
    }
}

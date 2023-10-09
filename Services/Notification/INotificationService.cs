﻿using Domain.Entities;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Notification
{
    public interface INotificationService :IService
    {
        Task<Domain.Entities.Notification> Create(Domain.Entities.Notification notification);
        Task<IList<Domain.Entities.Notification>> GetNotificationDataByUserId(int userId);
        Task<Domain.Entities.Notification> GetById(int id);
        Task<int> GetUnreadNotificationCountByUserId(int userId);
    }
}
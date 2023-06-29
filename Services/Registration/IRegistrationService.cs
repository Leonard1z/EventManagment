using Domain.Entities;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Registration
{
    public interface IRegistrationService : IService
    {
        bool Delete(int id);
        Task<bool> CheckIfUserExist(int userId, int eventId);
        Task RegisterUserForEvent(int userId, int eventId);
        IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId);
    }
}

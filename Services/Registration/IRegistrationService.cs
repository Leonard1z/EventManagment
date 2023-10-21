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
        Task<bool> IsUserRegisteredAsync(int userId, int eventId, int ticketTypeId);
        Task RegisterUserForEventAsync(Domain.Entities.Registration registration);
        IEnumerable<Domain.Entities.Registration> GetRegistrationByEventId(int eventId);
    }
}

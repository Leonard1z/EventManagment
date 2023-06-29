using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Registrations
{
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(EventManagmentDb context) : base(context)
        {

        }

        public async Task<Registration> GetUserAndEvent(int userId, int eventId)
        {
            return await DbSet.FirstOrDefaultAsync(r => r.UserAccountId == userId && r.EventId == eventId);
        }
        public IEnumerable<Registration> GetRegistrationByEventId(int eventId)
        {
            return DbSet.Where(r => r.EventId == eventId).ToList();
        }

    }
}

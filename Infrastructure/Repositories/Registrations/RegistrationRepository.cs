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

        public async Task<bool> IsUserRegisteredAsync(int userId, int eventId, int ticketTypeId)
        {
            return await DbSet.AnyAsync(r=>r.UserAccountId == userId && r.EventId == eventId && r.TicketTypeId == ticketTypeId);
        }
        public IEnumerable<Registration> GetRegistrationByEventId(int eventId)
        {
            return DbSet.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<List<Registration>> GetUserPurchasedTicketsAsync(int userId)
        {
            return await DbSet.Include(r=>r.TicketType)
                .Include(r=>r.Event)
                .Where(r=>r.UserAccountId == userId)
                .ToListAsync();
        }

        public async Task<Registration> GetRegistrationById(int id)
        {
            return await DbSet
                .Include(r => r.Event)
                .Include(r => r.TicketType)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<int> GetTotalTicketsSoldForUser(int userId)
        {
            var totalTicketsSold = await DbSet.Where(r => r.Event.UserAccountId == userId)
                .SumAsync(r => r.Quantity);

            return totalTicketsSold;
        }

        public async Task<int> GetTotalTicketsSoldForAdmin()
        {
            var totalTicketsSold = await DbSet.SumAsync(r => r.Quantity);

            return totalTicketsSold;
        }

        public async Task<ICollection<AssignedTicket>> GetAssignedTicketsForEvent(int eventId)
        {
            var registrationForEvent = await DbSet.Include(r => r.AssignedTickets)
                .Where(r => r.EventId == eventId)
                .ToListAsync();

            var assignedTickets = registrationForEvent
                .SelectMany(r => r.AssignedTickets
                    .Select(t => new AssignedTicket
                    {
                        Id = t.Id,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Email = t.Email,
                        PhoneNumber = t.PhoneNumber,

                    }))
                .ToList();

            return assignedTickets;
        }
    }
}

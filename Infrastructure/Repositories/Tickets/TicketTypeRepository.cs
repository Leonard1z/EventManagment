using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Tickets
{
    public class TicketTypeRepository : GenericRepository<TicketType>, ITicketTypeRepository
    {
        public TicketTypeRepository(EventManagmentDb context) : base(context)
        {
        }

        public async Task<TicketType> GetTicketByIdAsync(int ticketId)
        {
            return await DbSet.FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<List<TicketType>> GetTicketsByEventId(int eventId)
        {
            return await DbSet
                .Include(x=>x.Registrations)
                .Include(x=>x.Reservations)
                .Where(x => x.EventId == eventId).ToListAsync();
        }
        public async Task<int> GetAvailableQuantity(int ticketId)
        {
            var ticket = await DbSet.FindAsync(ticketId);
            if (ticket != null)
            {
                return ticket.Quantity;
            }
            return 0;
        }

        public bool HasRegistrations(int ticketId)
        {
            return DbSet.Any(t => t.Id == ticketId && t.Registrations.Any());
        }

        public bool HasReservations(int ticketId)
        {
            return DbSet.Any(t => t.Id == ticketId && t.Reservations.Any());
        }
    }
}

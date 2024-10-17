using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Tickets
{
    public interface ITicketTypeRepository : IGenericRepository<TicketType>
    {
        Task<List<TicketType>> GetTicketsByEventId(int eventId);
        Task<TicketType> GetTicketByIdAsync(int ticketId);
        Task<int> GetAvailableQuantity(int ticketId);
        bool HasRegistrations(int ticketId);
        bool HasReservations(int ticketId);

    }
}

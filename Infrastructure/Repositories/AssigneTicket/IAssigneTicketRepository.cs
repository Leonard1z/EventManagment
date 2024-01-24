using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AssigneTicket
{
    public interface IAssigneTicketRepository : IGenericRepository<AssignedTicket>
    {
        Task<AssignedTicket> GetTicketByTicketNumberAsync(int ticketNumber);
    }
}

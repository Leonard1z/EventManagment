using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AssigneTicket
{
    public class AssigneTicketRepository : GenericRepository<AssignedTicket>, IAssigneTicketRepository
    {
        public AssigneTicketRepository(EventManagmentDb context) : base(context)
        {

        }

        public async Task<AssignedTicket> GetTicketByTicketNumberAsync(int ticketNumber)
        {
            return await DbSet.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
        }
    }
}

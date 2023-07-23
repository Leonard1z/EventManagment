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

        public async Task<List<TicketType>> GetTicketsByEventId(int eventId)
        {
            return await DbSet.Where(x => x.EventId == eventId).ToListAsync();
        }
    }
}

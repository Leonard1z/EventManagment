using Domain.Entities;
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
    }
}

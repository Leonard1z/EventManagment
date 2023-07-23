using Domain._DTO.Ticket;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tickets
{
    public interface ITicketTypeService : IService
    {
        Task<List<TicketTypeDto>> GetTicketsByEventId(int eventId);
    }
}

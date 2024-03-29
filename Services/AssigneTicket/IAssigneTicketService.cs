﻿using Domain.Entities;
using Services.Common;

namespace Services.AssigneTicket
{
    public interface IAssigneTicketService : IService
    {
        Task<bool> AssignTicketsAsync(List<AssignedTicket> assigneeTicket);
        Task<AssignedTicket> GetTicketByTicketNumberAsync(int ticketNumber);
        Task UpdateAsync(AssignedTicket ticket);
    }
}

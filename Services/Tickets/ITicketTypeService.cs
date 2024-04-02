using Domain._DTO.Ticket;
using Services.Common;

namespace Services.Tickets
{
    public interface ITicketTypeService : IService
    {
        Task<List<TicketTypeDto>> GetTicketsByEventId(int eventId);
        Task<TicketTypeDto> GetTicketByIdAsync(int ticketId);
        Task<int> GetAvailableQuantity(int ticketId);
        TicketTypeDto AddTicket(TicketTypeDto ticket);
        Task<TicketTypeDto> UpdateAsync(TicketTypeDto ticketTypeDto);
        bool Delete(int id);
        bool HasRegistrations(int ticketId);
        bool HasReservations(int ticketId);
    }
}

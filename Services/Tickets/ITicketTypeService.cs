using Domain._DTO.Ticket;
using Services.Common;

namespace Services.Tickets
{
    public interface ITicketTypeService : IService
    {
        Task<List<TicketTypeDto>> GetTicketsByEventId(int eventId);
        Task<TicketTypeDto> GetTicketByIdAsync(int ticketId);
        Task<TicketTypeEditDto> GetTicketForEditByIdAsync(int ticketId);
        Task<int> GetAvailableQuantity(int ticketId);
        TicketTypeDto AddTicket(TicketTypeDto ticket);
        Task<TicketTypeEditDto> UpdateAsync(TicketTypeEditDto ticketTypeEditDto);
        Task<TicketTypeDto> GetById(int id);
        bool Delete(int id);
        bool HasRegistrations(int ticketId);
        bool HasReservations(int ticketId);
    }
}

using Domain._DTO.Event;
using Domain._DTO.Ticket;

namespace Domain.ViewModels
{
    public class EventDetailsViewModel
    {
        public EventDto Event { get; set; }
        public List<TicketTypeDto> Tickets { get; set; }
    }
}

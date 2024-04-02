using AutoMapper;
using Domain._DTO.Ticket;
using Domain.Entities;
using Infrastructure.Repositories.Tickets;
using static StackExchange.Redis.Role;

namespace Services.Tickets
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly ITicketTypeRepository _ticketTypesRepository;
        private readonly IMapper _mapper;

        public TicketTypeService(ITicketTypeRepository ticketTypesRepository, IMapper mapper)
        {
            _ticketTypesRepository = ticketTypesRepository;
            _mapper = mapper;
        }

        public async Task<TicketTypeDto> GetTicketByIdAsync(int ticketId)
        {
            var result = await _ticketTypesRepository.GetTicketByIdAsync(ticketId);

            return _mapper.Map<TicketTypeDto>(result);
        }

        public async Task<List<TicketTypeDto>> GetTicketsByEventId(int eventId)
        {
            var result = await _ticketTypesRepository.GetTicketsByEventId(eventId);

            int activeReservationsCount;

            var tickets = result.Select(t => new TicketTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Price = t.Price,
                Quantity = t.Quantity,
                IsAvailable = t.IsAvailable,
                IsFree= t.IsFree,
                SaleStartDate = t.SaleStartDate,
                SaleEndDate = t.SaleEndDate,
                TotalTickets = activeReservationsCount = t.Reservations.Where(r => r.Status == ReservationStatus.Active || r.Status == ReservationStatus.PaymentInProgress).Sum(r=>r.Quantity)
                + t.Quantity + t.Registrations.Sum(r=>r.Quantity),

            }).ToList();

            return tickets;
           
        }
        public async Task<int> GetAvailableQuantity(int ticketId)
        {
            return await _ticketTypesRepository.GetAvailableQuantity(ticketId);
        }

        public TicketTypeDto AddTicket(TicketTypeDto ticket)
        {
            var result = _ticketTypesRepository.Create(_mapper.Map<TicketType>(ticket));

            return _mapper.Map<TicketTypeDto>(result);
        }

        public async Task<TicketTypeDto> UpdateAsync(TicketTypeDto ticketTypeDto)
        {
            var result = await _ticketTypesRepository.UpdateAsync(_mapper.Map<TicketType>(ticketTypeDto));

            return _mapper.Map<TicketTypeDto>(result);
        }

        public bool Delete(int id)
        {
            _ticketTypesRepository.Delete(id);
            return true;
        }

        public bool HasRegistrations(int ticketId)
        {
            return _ticketTypesRepository.HasRegistrations(ticketId);
        }

        public bool HasReservations(int ticketId)
        {
            return _ticketTypesRepository.HasReservations(ticketId);
        }
    }
}

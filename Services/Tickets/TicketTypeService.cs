using AutoMapper;
using Domain._DTO.Ticket;
using Infrastructure.Repositories.Tickets;

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

            return _mapper.Map<List<TicketTypeDto>>(result);
        }
        public async Task<int> GetAvailableQuantity(int ticketId)
        {
            return await _ticketTypesRepository.GetAvailableQuantity(ticketId);
        }
    }
}

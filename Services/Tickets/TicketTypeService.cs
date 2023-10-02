using AutoMapper;
using Domain._DTO.Ticket;
using Domain.Entities;
using Infrastructure.Repositories.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

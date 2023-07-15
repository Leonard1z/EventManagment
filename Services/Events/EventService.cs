using AutoMapper;
using Domain._DTO.Event;
using Domain.Entities;
using Infrastructure.Repositories.Events;

namespace Services.Events
{
    public class EventService : IEventService
    {
        public readonly IEventRepository _eventRepository;
        public readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }
        public IQueryable<EventDto> GetAllForPagination(string filter, string encryptedId)
        {
            var result = _eventRepository.GetAllForPagination(filter, encryptedId);

            return _mapper.ProjectTo<EventDto>(result);
        }

        public EventCreateDto Create(EventCreateDto eventCreateDto)
        {

            eventCreateDto.IsActive = true;
            var result = _eventRepository.Create(_mapper.Map<Event>(eventCreateDto));

            return _mapper.Map<EventCreateDto>(result);
        }

        public async Task<IEnumerable<EventDto>> GetAll()
        {

            var result = await _eventRepository.GetAll();

            return _mapper.Map<List<EventDto>>(result.ToList());
        }

        public async Task<IEnumerable<EventDto>> GetAllEvents()
        {

            var result = await _eventRepository.GetAllEvents();

            return _mapper.Map<List<EventDto>>(result.ToList());
        }

        public async Task<EventDto> GetByIdWithCategory(int id)
        {
            return _mapper.Map<EventDto>(await _eventRepository.GetByIdWithCategory(id));
        }

        public async Task<EventEditDto> GetByIdEdit(int id)
        {
            var result = await _eventRepository.GetById(id);

            return _mapper.Map<EventEditDto>(result);
        }

        public EventEditDto Update(EventEditDto eventEditDto)
        {
            var result = _eventRepository.Update(_mapper.Map<Event>(eventEditDto));

            return _mapper.Map<EventEditDto>(result);
        }
        public bool Delete(int id)
        {
            _eventRepository.Delete(id);

            return true;
        }

        public async Task<IEnumerable<EventDto>> GetUserEvents(int useId)
        {
            var result = await _eventRepository.GetUserEvents(useId);

            return _mapper.Map<List<EventDto>>(result.ToList());
        }

        public IEnumerable<EventDto> GetExpiredEvents()
        {
            var result = _eventRepository.GetExpiredEvents();

            return _mapper.Map<IEnumerable<EventDto>>(result);
        }

        public async Task<EventDto> GetById(int eventId)
        {
            var eventEntity = await _eventRepository.GetById(eventId);
            if (eventEntity == null)
            {
                return null;
            }

            var eventDto = _mapper.Map<EventDto>(eventEntity);

            return eventDto;
        }

        public async Task<EventDto> UpdateByIsActive(EventDto eventDto)
        {
            var result = _eventRepository.Update(_mapper.Map<Event>(eventDto));

            return _mapper.Map<EventDto>(result);
        }
    }
}

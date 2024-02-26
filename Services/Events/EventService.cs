using AutoMapper;
using Domain._DTO.Category;
using Domain._DTO.Event;
using Domain._DTO.UserAccount;
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
            if (string.IsNullOrEmpty(eventCreateDto.Status))
            {
                eventCreateDto.Status = "Draft";
            }
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

            return _mapper.Map<List<EventDto>>(result.ToList(), opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    foreach (var item in dest)
                    {
                        item.UserAccount = new Domain._DTO.UserAccount.UserAccountDto
                        {
                            FirstName = item.UserAccount.FirstName,
                            Username = item.UserAccount.Username,
                            Email = item.UserAccount.Email
                        };
                    }
                });
            });
        }


        public async Task<IList<EventWithMetricsDto>> GetAllEventsWithSoldAndGross()
        {
            var events = await _eventRepository.GetAllEventsWithSoldAndGross();

            var result = events.Select(e => new EventWithMetricsDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Image = e.Image,
                City = e.City,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status=e.Status,
                StreetName=e.StreetName,
                IsActive=e.IsActive,
                IsFree=e.IsFree,
                Category = _mapper.Map<CategoryDto>(e.Category),
                UserAccount = new UserAccountDto
                {
                    FirstName = e.UserAccount.FirstName,
                    Username = e.UserAccount.Username,
                },
                Sold = e.Registrations.Sum(reg => reg.Quantity),
                Gross = e.Registrations.Sum(reg => reg.TotalPrice)
            }).ToList();

            return result;
        }

        public async Task<IList<EventWithMetricsDto>> GetActiveEventsWithSoldAndGrossForEventCreator(int useId)
        {
            var events = await _eventRepository.GetActiveEventsWithSoldAndGrossForEventCreator(useId);

            var result = events.Select(e => new EventWithMetricsDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Image = e.Image,
                City = e.City,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status,
                StreetName = e.StreetName,
                IsActive = e.IsActive,
                IsFree = e.IsFree,
                Category = _mapper.Map<CategoryDto>(e.Category),
                UserAccount = new UserAccountDto
                {
                    FirstName = e.UserAccount.FirstName,
                    Username = e.UserAccount.Username,
                },
                Sold = e.Registrations.Sum(reg => reg.Quantity),
                Gross = e.Registrations.Sum(reg => reg.TotalPrice)
            }).ToList();

            return result;

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
        public async Task<EventDto> UpdateAsync(EventDto eventDto)
        {
            var result = await _eventRepository.UpdateAsync(_mapper.Map<Event>(eventDto));

            return _mapper.Map<EventDto>(result);
        }
        public bool Delete(int id)
        {
            _eventRepository.Delete(id);

            return true;
        }

        public async Task<IEnumerable<EventDto>> GetActiveEventsForEventCreator(int useId)
        {
            var result = await _eventRepository.GetActiveEventsForEventCreator(useId);

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

        public async Task<IEnumerable<EventDto>> GetAllByIsActive()
        {
            var result = await _eventRepository.GetAllByIsActive();
            return _mapper.Map<List<EventDto>>(result.ToList());
        }

        public async Task<EventDto> GetEventDetails(int eventId)
        {
            var result = await _eventRepository.GetEventDetails(eventId);

            return _mapper.Map<EventDto>(result);
        }

        public async Task<int> GetTotalEventCount()
        {
            return await _eventRepository.GetTotalEventCount();
        }

        public async Task<int> GetTotalEventCountForEventCreator(int eventCreatorId)
        {
            return await _eventRepository.GetTotalEventCountForEventCreator(eventCreatorId);
        }
        public async Task<IList<EventDto>> GetUpcomingEventsForAdmin()
        {
            DateTime currentDate = DateTime.Now;

            var upcomingEvents = await _eventRepository.GetUpcomingEventsForAdmin(currentDate);

            return _mapper.Map<IList<EventDto>>(upcomingEvents);
        }

        public async Task<IList<EventDto>> GetUpcomingEvents(int userId)
        {
            DateTime currentDate = DateTime.Now;

            var upcomingEvents = await _eventRepository.GetUpcomingEvents(userId, currentDate);

            return _mapper.Map<IList<EventDto>>(upcomingEvents);

        }

        public async Task<int> GetTotalUpcomingEventsForEventCreator(int eventCreatorId)
        {
            DateTime currentDate = DateTime.Now;

            var totalUpcomingEvents = await _eventRepository.GetTotalUpcomingEventsForEventCreator(eventCreatorId, currentDate);

            return totalUpcomingEvents;
        }

        public async Task<int> GetTotalUpcomingEventsForAdmin()
        {
            DateTime currentDate = DateTime.Now;

            var totalUpcomingEvents = await _eventRepository.GetTotalUpcomingEventsForAdmin(currentDate);

            return totalUpcomingEvents;
        }

        public async Task<IList<EventDto>> GetUpcomingEventsWithinOneWeek(int userId)
        {
            DateTime currentDate = DateTime.Now;
            DateTime oneWeekLater = currentDate.AddDays(7);

            var upcomingEvents = await _eventRepository.GetUpcomingEventsWithinOneWeek(currentDate, oneWeekLater,userId);

            return _mapper.Map<IList<EventDto>>(upcomingEvents);
        }

        public async Task<EventDto> GetByIdEditForEditStatus(int id)
        {
            return _mapper.Map<EventDto>(await _eventRepository.GetByIdEditForEditStatus(id));
        }
    }
}

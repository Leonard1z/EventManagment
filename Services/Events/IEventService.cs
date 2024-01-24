using Domain._DTO.Event;
using Services.Common;

namespace Services.Events
{
    public interface IEventService : IService
    {
        IQueryable<EventDto> GetAllForPagination(string filter, string? encryptedId);
        EventCreateDto Create(EventCreateDto eventCreateDto);
        bool Delete(int id);
        Task<IEnumerable<EventDto>> GetAll();
        Task<IEnumerable<EventDto>> GetAllByIsActive();
        Task<EventDto> GetByIdWithCategory(int id);
        Task<EventDto> GetById(int id);
        Task<EventEditDto> GetByIdEdit(int id);
        Task<IEnumerable<EventDto>> GetAllEvents();
        Task<IEnumerable<EventDto>> GetActiveEventsForEventCreator(int useId);
        EventEditDto Update(EventEditDto eventEditDto);
        Task<EventDto> UpdateByIsActive(EventDto eventDto);
        IEnumerable<EventDto> GetExpiredEvents();
        Task<EventDto> GetEventDetails(int eventId);
        Task<int> GetTotalEventCount();
        Task<int> GetTotalEventCountForEventCreator(int eventCreatorId);
        Task<int> GetTotalUpcomingEventsForAdmin();
        Task<int> GetTotalUpcomingEventsForEventCreator(int eventCreatorId);
        Task<IList<EventDto>> GetUpcomingEventsForAdmin();
        Task<IList<EventDto>> GetUpcomingEvents(int userId);
        Task<IList<EventDto>> GetUpcomingEventsWithinOneWeek(int userId);
    }
}

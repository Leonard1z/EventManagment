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
        Task<IEnumerable<EventDto>> GetUserEvents(int useId);
        EventEditDto Update(EventEditDto eventEditDto);
        Task<EventDto> UpdateByIsActive(EventDto eventDto);
        IEnumerable<EventDto> GetExpiredEvents();
        Task<EventDto> GetEventDetails(int eventId);
    }
}

using Domain.Entities;

namespace Infrastructure.Repositories.Events
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        IQueryable<Event> GetAllForPagination(string filter, string encryptedId);
        Task<IList<Event>> GetAllEvents();
        Task<IList<Event>> GetAllEventsWithSoldAndGross();
        Task<IList<Event>> GetActiveEventsWithSoldAndGrossForEventCreator(int userId);
        Task<IEnumerable<Event>> GetAllByIsActive();
        Task<Event> GetByIdWithCategory(int id);
        IEnumerable<Event> GetExpiredEvents();
        Task<IEnumerable<Event>> GetActiveEventsForEventCreator(int userId);
        Task<Event> GetEventDetails(int eventId);
        Task<int> GetTotalEventCount();
        Task<int> GetTotalEventCountForEventCreator(int eventCreatorId);
        Task<int> GetTotalUpcomingEventsForAdmin(DateTime currentDate);
        Task<int> GetTotalUpcomingEventsForEventCreator(int eventCreatorId, DateTime currentDate);
        Task<IList<Event>> GetUpcomingEventsForAdmin(DateTime currentDate);
        Task<IList<Event>> GetUpcomingEvents(int userId, DateTime currentDate);
        Task<IList<Event>> GetUpcomingEventsWithinOneWeek(DateTime currentDate, DateTime oneWeekLater,int userId);
    }
}

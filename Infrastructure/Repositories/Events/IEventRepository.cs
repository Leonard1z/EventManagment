using Domain.Entities;

namespace Infrastructure.Repositories.Events
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        IQueryable<Event> GetAllForPagination(string filter, string encryptedId);
        Task<IList<Event>> GetAllEvents();
        Task<Event> GetByIdWithCategory(int id);
        IEnumerable<Event> GetExpiredEvents();
        Task<IEnumerable<Event>> GetUserEvents(int userId);
    }
}

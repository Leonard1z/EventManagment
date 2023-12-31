﻿using Domain.Entities;

namespace Infrastructure.Repositories.Events
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        IQueryable<Event> GetAllForPagination(string filter, string encryptedId);
        Task<IList<Event>> GetAllEvents();
        Task<IEnumerable<Event>> GetAllByIsActive();
        Task<Event> GetByIdWithCategory(int id);
        IEnumerable<Event> GetExpiredEvents();
        Task<IEnumerable<Event>> GetUserEvents(int userId);
        Task<Event> GetEventDetails(int eventId);
    }
}

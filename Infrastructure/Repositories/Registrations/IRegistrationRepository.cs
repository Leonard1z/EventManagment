using Domain.Entities;

namespace Infrastructure.Repositories.Registrations
{
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task<Registration> GetUserAndEvent(int userId, int eventId);
        IEnumerable<Registration> GetRegistrationByEventId(int eventId);
    }
}

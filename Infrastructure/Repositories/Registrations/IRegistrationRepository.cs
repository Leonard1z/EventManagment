using Domain.Entities;

namespace Infrastructure.Repositories.Registrations
{
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        Task<bool> IsUserRegisteredAsync(int userId, int eventId,int ticketTypeId);
        IEnumerable<Registration> GetRegistrationByEventId(int eventId);
        Task<List<Registration>> GetUserPurchasedTicketsAsync(int userId);
        Task<Registration> GetRegistrationById(int id);
        Task<int> GetTotalTicketsSoldForUser(int userId);
        Task<int> GetTotalTicketsSoldForAdmin();

    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Reservations
{
    public class ReservationRepository:GenericRepository<Reservation>, IReservationRepository
    {

        public ReservationRepository(EventManagmentDb context) : base(context)
        {

        }

        public async Task<bool> ExistsByReservationNumber(int reservationNumber)
        {
            return await DbSet.AnyAsync(r => r.ReservationNumber == reservationNumber);
        }

        public async Task<IList<Reservation>> GetExpiredReservationsAsync(DateTime currentDate)
        {
            return await DbSet.Where(r => r.ExpirationTime <= currentDate && r.Status != ReservationStatus.Expired)
                              .AsNoTracking()
                              .ToListAsync();
        }
        public async Task<Reservation> GetByIdWithTicket(int id, Func<IQueryable<Reservation>, IQueryable<Reservation>> include = null)
        {
            var query = DbSet.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> HasActiveReservationForTickets(int userId, List<int> ticketIds)
        {
            return await DbSet.AnyAsync(r => r.UserAccountId == userId && r.Status == ReservationStatus.Active && ticketIds.Contains(r.TicketTypeId));
        }
        public async Task<bool> HasCompletedPayment(int userId, int eventId, int ticketId)
        {
            var hasCompletedPayment = await DbSet
                .AnyAsync(r => r.UserAccountId == userId && r.EventId == eventId && r.TicketTypeId == ticketId && r.Status == ReservationStatus.Paid);

            return hasCompletedPayment;
        }
    }
}

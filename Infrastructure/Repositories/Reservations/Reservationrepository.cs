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
    }
}

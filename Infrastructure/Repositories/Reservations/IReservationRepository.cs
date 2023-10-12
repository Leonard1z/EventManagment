using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Reservations
{
    public interface IReservationRepository:IGenericRepository<Reservation>
    {
        Task<IList<Reservation>> GetExpiredReservationsAsync(DateTime currentDate);
        Task<bool> ExistsByReservationNumber(int reservationNumber);
    }
}

using Domain.Entities;
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
    }
}

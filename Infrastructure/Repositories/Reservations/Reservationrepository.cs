using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Reservations
{
    public class Reservationrepository:GenericRepository<Reservation>, IReservationRepository
    {

        public Reservationrepository(EventManagmentDb context) : base(context)
        {

        }
    }
}

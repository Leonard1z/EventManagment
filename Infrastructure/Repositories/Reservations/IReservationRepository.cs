﻿using Domain.Entities;
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
        Task<Reservation> GetByIdWithTicket(int id, Func<IQueryable<Reservation>, IQueryable<Reservation>> include = null);
        Task<bool> HasActiveReservationForTickets(int userId, List<int> ticketIds);
        Task<bool> HasCompletedPayment(int userId, int eventId, int ticketId);
    }
}

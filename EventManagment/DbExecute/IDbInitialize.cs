using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbExecute
{
    public interface IDbInitialize
    {
        Task DbExecute();
        Task CreateAdmin();
        void DeleteExpiredEvents();
        Task UpdateTicketAvailability();
        Task CheckAndUpdateExpiredReservation();
    }
}

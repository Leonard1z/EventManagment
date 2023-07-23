using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbExecute
{
    public interface IDbInitialize
    {
        void DbExecute();
        Task CreateAdmin();
        void DeleteExpiredEvents();
        Task UpdateTicketAvailability();
    }
}

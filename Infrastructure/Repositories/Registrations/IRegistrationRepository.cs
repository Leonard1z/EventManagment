using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Registrations
{
    public interface IRegistrationRepository : IGenericRepository<Registration>
    {
        IQueryable<Registration> GetAllForPagination(string filter);
        Task<IList<Registration>> GetAllRegistration();
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Registrations
{
    public class RegistrationRepository : GenericRepository<Registration>, IRegistrationRepository
    {
        public RegistrationRepository(EventManagmentDb context) : base(context)
        {
        }

        public IQueryable<Registration> GetAllForPagination(string filter)
        {
            var result = DbSet.Include(x => x.UserAccount)
                .Include(x => x.Event)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                result = result.Where(x => x.Id.ToString().Contains(filter));
            }
            return result;
        }
        public async Task<IList<Registration>> GetAllRegistration()
        {
            var result = DbSet.Include(x => x.UserAccount)
                .Include(x => x.Event)
                .AsNoTracking()
                .AsQueryable();

            return await result.ToListAsync();
        }

    }
}

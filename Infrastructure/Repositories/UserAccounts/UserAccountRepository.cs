using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.UserAccounts
{
    public class UserAccountRepository : GenericRepository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(EventManagmentDb context) : base(context)
        {
        }

        public IQueryable<UserAccount> GetAllForPagination(string filter)
        {
            var result = DbSet.Include(x => x.Events)
                             .Include(x => x.Registrations)
                             .AsNoTracking()
                             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                result = result.Where(x => x.FirstName.Contains(filter));
            }
            return result;
        }
        public async Task<IList<UserAccount>> GetAllUserAccounts()
        {
            var result = DbSet.Include(x => x.Events)
                              .Include(x => x.Registrations)
                              .AsNoTracking()
                              .AsQueryable();

            return await result.ToListAsync();
        }
        public bool CheckIfEmailExist(string email)
        {
            var result = DbSet.Any(x => x.Email == email);

            return result;
        }

        public bool CheckIfUserExist(string username)
        {
            var result = DbSet.Any(x => x.FirstName == username);

            return result;
        }
        public UserAccount Login(string username, string password)
        {
            throw new NotImplementedException();
        }



    }
}

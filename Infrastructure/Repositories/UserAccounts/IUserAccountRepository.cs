using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.UserAccounts
{
    public interface IUserAccountRepository : IGenericRepository<UserAccount>
    {
        IQueryable<UserAccount> GetAllForPagination(string filter);
        Task<IList<UserAccount>> GetAllUserAccounts();
        UserAccount Login(string username, string password);
        bool CheckIfEmailExist(string email);
        bool CheckIfUserExist(string username);
    }
}

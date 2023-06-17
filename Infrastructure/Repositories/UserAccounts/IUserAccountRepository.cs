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
        Task<IList<UserAccount>> GetAllUserAccounts();
        bool CheckIfEmailExist(string email);
        bool CheckIfUserExist(string username);
    }
}

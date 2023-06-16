using Domain._DTO.Category;
using Domain._DTO.UserAccount;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UserAccount
{
    public interface IUserAccountService : IService
    {
        Task<IEnumerable<UserAccountDto>> GetAll();
    }
}

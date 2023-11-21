using Domain._DTO.UserAccount;
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
        public async Task<IList<UserAccount>> GetAllUserAccounts()
        {
            var result = DbSet.Include(x => x.Role)
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
            var result = DbSet.Any(x => x.Username == username);

            return result;
        }

        public UserAccount GetByEmail(string email)
        {
            var user = DbSet.FirstOrDefault(u => u.Email == email);

            return user;
        }
        public async Task<UserAccount> GetAdminByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserAccount> GetUserByVerificationToken(string token)
        {
            var user = await DbSet.SingleOrDefaultAsync(u => u.EmailVerificationToken == token);

            return user;
        }

        public async Task<UserAccount> GetUserByPasswordResetToken(string token)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.PasswordResetToken == token);
        }
    }
}

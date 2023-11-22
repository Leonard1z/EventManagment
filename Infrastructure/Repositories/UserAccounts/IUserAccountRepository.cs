using Domain._DTO.UserAccount;
using Domain.Entities;

namespace Infrastructure.Repositories.UserAccounts
{
    public interface IUserAccountRepository : IGenericRepository<UserAccount>
    {
        Task<IList<UserAccount>> GetAllUserAccounts();
        bool CheckIfEmailExist(string email);
        bool CheckIfUserExist(string username);
        UserAccount GetByEmail(string email);
        Task<UserAccount> GetAdminByEmail(string email);
        Task<UserAccount> GetUserByVerificationToken(string token);
        Task<UserAccount> GetUserByPasswordResetToken(string token);
        IQueryable<UserAccount> GetAllForPagination(string filter, string encryptedId);
    }
}

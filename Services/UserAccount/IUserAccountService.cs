using Domain._DTO.UserAccount;
using Services.Common;

namespace Services.UserAccount
{
    public interface IUserAccountService : IService
    {
        Task<IEnumerable<UserAccountDto>> GetAll();
        bool CheckIfUserExist(string username);
        bool CheckIfEmailExist(string email);
        UserAccountCreateDto Create(UserAccountCreateDto userAccountCreateDto);
        UserAccountDto Authenticate(LoginDto loginDto);
        Task<UserAccountDto> GetAdminByEmail(string email);
        Task<Domain.Entities.UserAccount> GetUserByVerificationToken(string token);
        Domain.Entities.UserAccount Update(Domain.Entities.UserAccount userAccount);
        string GenerateVerificationToken();
        Task SendEmailVerificationAsync(string email, string firstName, string verificationUrl);
        Task<UserAccountDto> GetByEmail(string email);
        string GeneratePasswordResetToken(string email);
        Task SendPasswordResetEmail(string email, string resetUrl);
        Task<Domain.Entities.UserAccount> GetUserByPasswordResetToken(string token);
        void ResetPasword(Domain.Entities.UserAccount userAccount, string newPassword);
        Task<IEnumerable<UserAccountDto>> GetAllUserAccountsAndRoles();
        IQueryable<UserAccountDto> GetAllForPagination(string filter, string? encryptedId);
        Task<UserAccountEditDto> GetByIdEdit(int id);
        UserAccountEditDto UpdateWithRole(UserAccountEditDto userAccountEditDto);
        Task<ProfileUpdateDto> GetProfileById(int userId);
        Task<ProfileUpdateDto> UpdateUserProfile(ProfileUpdateDto profileUpdateDto);

    }
}

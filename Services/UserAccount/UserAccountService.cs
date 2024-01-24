using AutoMapper;
using Domain._DTO.UserAccount;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.AspNetCore.Hosting;
using Services.Security;
using Services.SendEmail;

namespace Services.UserAccount
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserAccountService(IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository,
            IEmailService emailService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
            _emailService = emailService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IEnumerable<UserAccountDto>> GetAll()
        {
            var result = await _userAccountRepository.GetAll();

            return _mapper.Map<List<UserAccountDto>>(result.ToList());
        }
        public bool CheckIfUserExist(string username)
        {
            return _userAccountRepository.CheckIfUserExist(username);
        }
        public bool CheckIfEmailExist(string email)
        {
            return _userAccountRepository.CheckIfEmailExist(email);
        }
        public UserAccountCreateDto Create(UserAccountCreateDto userAccountCreateDto)
        {
            string salt;
            userAccountCreateDto.Password = PasswordHasher.HashPassword(userAccountCreateDto.Password, out salt);
            userAccountCreateDto.Salt = salt;

            var result = _userAccountRepository.Create(_mapper.Map<Domain.Entities.UserAccount>(userAccountCreateDto));

            return _mapper.Map<UserAccountCreateDto>(result);
        }

        public UserAccountDto Authenticate(LoginDto loginDto)
        {
            var user = _userAccountRepository.GetByEmail(loginDto.Email);

            if (user != null)
            {
                var role = _roleRepository.GetRoleById(user.RoleId);
                user.Role = role;
            }

            if (user != null && PasswordHasher.VerifyPassword(loginDto.Password, user.Password, user.Salt))
            {
                var userDto = _mapper.Map<UserAccountDto>(user);

                return userDto;
            }


            return null;
        }

        public async Task<UserAccountDto> GetAdminByEmail(string email)
        {
            var result = await _userAccountRepository.GetAdminByEmail(email);

            return _mapper.Map<UserAccountDto>(result);
        }

        public async Task<Domain.Entities.UserAccount> GetUserByVerificationToken(string token)
        {
            var result = await _userAccountRepository.GetUserByVerificationToken(token);

            return result;
        }

        public Domain.Entities.UserAccount Update(Domain.Entities.UserAccount userAccount)
        {
            var result = _userAccountRepository.Update(userAccount);

            return result;
        }
        public string GenerateVerificationToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var token = new string(Enumerable.Repeat(chars, 32)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return token;
        }
        public async Task SendEmailVerificationAsync(string email, string firstName, string verificationUrl)
        {
            var pathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
            + Path.DirectorySeparatorChar.ToString() + "EmailVerification.html";

            //string body = $"<p>Plese click the link to verify the email <a href={verificationUrl}>Verify Email</a></p>";
            string subject = "Confirm Your Emaiil";
            string tittle = "Confirm Account Registration";
            string message = "Thank you for showing your interest in our website. All you need to do is click the button below, to verify your account.";
            string body = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                body = streamReader.ReadToEnd();
            }

            string messageBody = string.Format(body, tittle, string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now), firstName, message, verificationUrl);

            try
            {
                await _emailService.SendEmailAsync(email, subject, messageBody);
            }
            catch
            {
                throw new Exception("Failed to send verification email. Please try again later.");
            }
        }

        public async Task<UserAccountDto> GetByEmail(string email)
        {
            var user = _userAccountRepository.GetByEmail(email);
            return _mapper.Map<UserAccountDto>(user);
        }

        public string GeneratePasswordResetToken(string email)
        {
            var userAccount = _userAccountRepository.GetByEmail(email);
            if (userAccount == null)
            {
                throw new Exception("User not found.");
            }

            string resetToken = Guid.NewGuid().ToString();
            DateTime tokenExpiry = DateTime.Now.AddHours(1);

            userAccount.PasswordResetToken = resetToken;
            userAccount.PasswordResetTokenExpiry = tokenExpiry;
            _userAccountRepository.Update(userAccount);

            return resetToken;
        }

        public async Task SendPasswordResetEmail(string email, string resetUrl)
        {
            var user = _userAccountRepository.GetByEmail(email);
            if (user != null)
            {
                var pathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                    + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates" +
                    Path.DirectorySeparatorChar.ToString() + "PasswordReset.html";

                string subject = "Password Reset Request";
                string tittle = "Password Reset";
                string message = "You are receiving this email because we received a password reset request for your account.";
                string body = "";

                using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
                {
                    body = streamReader.ReadToEnd();
                }

                //tittle{0}
                //DateTime{1}
                //name{2}
                //message{3}
                //resetUrl{4}

                string messageBody = string.Format(body, tittle, string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now), user.FirstName, message, resetUrl);

                try
                {
                    await _emailService.SendEmailAsync(email, subject, messageBody);
                }
                catch
                {
                    throw new Exception("Failed to send verification email. Please try again later.");
                }
            }
        }

        public async Task<Domain.Entities.UserAccount> GetUserByPasswordResetToken(string token)
        {
            var user = await _userAccountRepository.GetUserByPasswordResetToken(token);

            return user;
        }

        public void ResetPasword(Domain.Entities.UserAccount userAccount, string newPassword)
        {
            string salt;
            userAccount.Password = PasswordHasher.HashPassword(newPassword, out salt);
            userAccount.Salt = salt;
            userAccount.PasswordResetToken = null;
            userAccount.PasswordResetTokenExpiry = null;

            _userAccountRepository.Update(userAccount);
        }

        public async Task<IEnumerable<UserAccountDto>> GetAllUserAccountsAndRoles()
        {
            var users = await _userAccountRepository.GetAllUserAccounts();

            return _mapper.Map<List<UserAccountDto>>(users.ToList());
        }

        public IQueryable<UserAccountDto> GetAllForPagination(string filter, string? encryptedId)
        {
            var result = _userAccountRepository.GetAllForPagination(filter, encryptedId);

            var result2 = _mapper.ProjectTo<UserAccountDto>(result);

            return result2;
        }

        public async Task<UserAccountEditDto> GetByIdEdit(int id)
        {
            var result = await _userAccountRepository.GetById(id);

            return _mapper.Map<UserAccountEditDto>(result);
        }

        public UserAccountEditDto UpdateWithRole(UserAccountEditDto userAccountEditDto)
        {
            if (string.IsNullOrEmpty(userAccountEditDto.Password))
            {
                // Properties not provided, exclude them from the update
                var result = _userAccountRepository.UpdateExceptProperties(
                    _mapper.Map<Domain.Entities.UserAccount>(userAccountEditDto),
                    u=>u.Password,
                    u=>u.Gender,
                    u=>u.IsEmailVerified,
                    u=>u.EmailVerificationToken,
                    u=>u.Salt,
                    u => u.PasswordResetToken,
                    u=>u.PasswordResetTokenExpiry,
                    u=>u.ProfileImage
                    );

                return _mapper.Map<UserAccountEditDto>(result);
            }
            else
            {
                var result = _userAccountRepository.Update(_mapper.Map<Domain.Entities.UserAccount>(userAccountEditDto));

                return _mapper.Map<UserAccountEditDto>(result);
            }
        }

        public async Task<ProfileUpdateDto> GetProfileById(int userId)
        {
            var user = await _userAccountRepository.GetById(userId);

            var profileUpdateDto = _mapper.Map<ProfileUpdateDto>(user);

            return profileUpdateDto;
        }

        public async Task<ProfileUpdateDto> UpdateUserProfile(ProfileUpdateDto profileUpdateDto)
        {

            // Properties not provided, exclude them from the update
            var result = _userAccountRepository.UpdateExceptProperties(
                _mapper.Map<Domain.Entities.UserAccount>(profileUpdateDto),
                u => u.Password,
                u => u.Gender,
                u => u.IsEmailVerified,
                u => u.EmailVerificationToken,
                u => u.Salt,
                u => u.PasswordResetToken,
                u => u.PasswordResetTokenExpiry
                );

            return _mapper.Map<ProfileUpdateDto>(result);

        }

        public bool VerifyPassword(string oldPassword, string password, string salt)
        {
            if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(oldPassword))
            {
                return false;
            }

            return PasswordHasher.VerifyPassword(oldPassword, password, salt);
        }

        public async Task ChangePassword(int userId, string newPassword)
        {
            var userAccount = await _userAccountRepository.GetById(userId);

            string salt;
            userAccount.Password = PasswordHasher.HashPassword(newPassword, out salt);
            userAccount.Salt = salt;

            _userAccountRepository.Update(userAccount);
        }
    }
}

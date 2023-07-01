using AutoMapper;
using Domain._DTO.UserAccount;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Template;
using RazorEngine.Templating;
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

        public UserAccountService(IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository,
            IEmailService emailService,
            IMapper mapper
            )
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
            _emailService = emailService;
            _mapper = mapper;
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
        public async Task SendEmailVerificationAsync(string email, string verificationUrl)
        {
            string subject = "Email Verification";
            string body = $"<p>Plese click the link to verify the email <a href={verificationUrl}>Verify Email</a></p>";

            try
            {
                await _emailService.SendEmailAsync(email, subject, body);
            }
            catch
            {
                throw new Exception("Failed to send verification email. Please try again later.");
            }
        }


    }
}

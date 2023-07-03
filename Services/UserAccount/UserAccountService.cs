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
            string message = "Thanks for Registering to our website were thankfoul to you for visiting our web";
            string body = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                body = streamReader.ReadToEnd();
            }


            string messageBody = string.Format(body, tittle, string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),firstName, message, verificationUrl);

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
}

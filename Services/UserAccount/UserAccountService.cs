using AutoMapper;
using Domain._DTO.Role;
using Domain._DTO.UserAccount;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.UserAccounts;
using Services.Security;

namespace Services.UserAccount
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserAccountService(IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository, IMapper mapper)
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
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
    }
}

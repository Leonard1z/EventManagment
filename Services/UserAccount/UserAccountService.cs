using AutoMapper;
using Domain._DTO.UserAccount;
using Infrastructure.Repositories.UserAccounts;
using Services.Security;

namespace Services.UserAccount
{
    public class UserAccountService : IUserAccountService
    {
        public readonly IUserAccountRepository _userAccountRepository;
        public readonly IMapper _mapper;

        public UserAccountService(IUserAccountRepository userAccountRepository, IMapper mapper)
        {
            _userAccountRepository = userAccountRepository;
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
    }
}

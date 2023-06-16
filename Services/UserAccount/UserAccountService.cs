using AutoMapper;
using Domain._DTO.Category;
using Domain._DTO.UserAccount;
using Infrastructure.Repositories.Events;
using Infrastructure.Repositories.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

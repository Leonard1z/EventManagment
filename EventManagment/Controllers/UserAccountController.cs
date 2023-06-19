using Domain._DTO.Event;
using Domain._DTO.UserAccount;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Security;
using Services.Role;
using Services.UserAccount;

namespace EventManagment.Controllers
{
    public class UserAccountController : Controller
    {

        private readonly IUserAccountService _userAccountService;
        private readonly IRoleService _roleService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<UserAccountController> _localizer;
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(IUserAccountService userAccountService,
            IRoleService roleService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<UserAccountController> localizer,
            ILogger<UserAccountController> logger)
        {
            _userAccountService = userAccountService;
            _roleService = roleService;
            _protector = provider.CreateProtector(purpose: purposeStrings.UserAccountControllerPs);
            _localizer = localizer;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("UserRegistration")]
        public ActionResult Register()
        {
            try
            {
                var dto = UserAccountCreate();

                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();


                return RedirectToAction(nameof(Register));
            }

        }

        [HttpPost]
        [Route("UserRegistration")]
        public ActionResult Register(UserAccountCreateDto userDto)
        {
            try
            {
                UserAccountCreateDescription(userDto);

                if (ModelState.IsValid)
                {

                    if (_userAccountService.CheckIfUserExist(userDto.FirstName))
                    {
                        TempData["message"] = "Error";
                        TempData["entity"] = _localizer["Ky emër përdoruesi është marrë tashmë, emri i përdoruesit duhet të jetë unik!"].ToString();
                        return RedirectToAction(nameof(Register));
                    }
                    else if (_userAccountService.CheckIfEmailExist(userDto.Email))
                    {

                        TempData["message"] = "Error";
                        TempData["entity"] = _localizer["Ky email është marrë tashmë, emaili duhet të jetë unike!"].ToString();
                        return RedirectToAction(nameof(Register));

                    }

                    _userAccountService.Create(userDto);

                    TempData["message"] = "Added";
                    TempData["entity"] = _localizer["User"].ToString();

                    return RedirectToAction(nameof(LogIn));
                }

                return RedirectToAction(nameof(LogIn));

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Register));
            }
        }

        public async Task<ActionResult> LogIn(UserAccount user)
        {
            return View();
        }

        private UserAccountCreateDto UserAccountCreateDescription(UserAccountCreateDto userAccountCreateDto)
        {
            userAccountCreateDto.FirstName = userAccountCreateDto.FirstName.Trim();
            userAccountCreateDto.RoleId = userAccountCreateDto.EncryptedRoleId != null ? int.Parse(_protector.Unprotect(userAccountCreateDto.EncryptedRoleId)) : 0;

            return userAccountCreateDto;
        }

        private UserAccountCreateDto UserAccountCreate()
        {
            return new UserAccountCreateDto()
            {
                Role = _roleService.GetAll().Result.Select(x =>
                {
                    x.EncryptedId = _protector.Protect(x.Id.ToString());
                    x.Id = 0;

                    return x;
                }).ToList()
            };

        }
    }
}

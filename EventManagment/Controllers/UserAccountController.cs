using Domain._DTO.Event;
using Domain._DTO.UserAccount;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Security;
using Services.Role;
using Services.UserAccount;
using System.Security.Claims;

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
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        [Route("UserRegistration")]
        public ActionResult Register()
        {
            try
            {
                return View();
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
                var defaultRole = _roleService.GetDefaultRole();
                userDto.RoleId = defaultRole.Id;

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

                    return RedirectToAction(nameof(Login));
                }

                return RedirectToAction(nameof(Login));

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Register));
            }
        }

        [HttpGet]
        [Route("Login")]
        public ActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();


                return RedirectToAction(nameof(Login));
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginDto loginDto, string returnUrl)
        {
            var userDto = _userAccountService.Authenticate(loginDto);

            if (userDto == null)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["Invalid username or password"].ToString();
                return RedirectToAction(nameof(Login));
            }

            TempData["message"] = "Success";
            TempData["entity"] = _localizer["Logged in successfully"].ToString();

            await SignInUserAsync(userDto);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(UserAccountDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.FirstName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });
        }


        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                TempData["message"] = "Success";
                TempData["entity"] = _localizer["Logged out successfully"].ToString();

                Response.Cookies.Delete(".AspNetCore.Cookies");

                return RedirectToAction(nameof(Login));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                return RedirectToAction(nameof(Login));
            }
        }

    }
}

using Domain._DTO.UserAccount;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Security;
using Services.Role;
using Services.UserAccount;
using System.Security.Claims;
using Services.SendEmail;

namespace EventManagment.Controllers
{
    public class UserAccountController : Controller
    {

        private readonly IUserAccountService _userAccountService;
        private readonly IRoleService _roleService;
        private readonly IEmailService _emailService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<UserAccountController> _localizer;
        private readonly ILogger<UserAccountController> _logger;

        public UserAccountController(IUserAccountService userAccountService,
            IRoleService roleService,
            IEmailService emailService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<UserAccountController> localizer,
            ILogger<UserAccountController> logger)
        {
            _userAccountService = userAccountService;
            _roleService = roleService;
            _emailService = emailService;
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
        public async Task<ActionResult> Register(UserAccountCreateDto userDto)
        {
            try
            {
                var defaultRole = _roleService.GetDefaultRole();
                userDto.RoleId = defaultRole.Id;

                if (_userAccountService.CheckIfUserExist(userDto.Username))
                {
                    TempData["message"] = "Error";
                    TempData["entity"] = _localizer["This username is already taken. Username must be unique!"].ToString();
                    return RedirectToAction(nameof(Register));
                }
                else if (_userAccountService.CheckIfEmailExist(userDto.Email))
                {

                    TempData["message"] = "Error";
                    TempData["entity"] = _localizer["This email is already taken. Emails must be unique!"].ToString();
                    return RedirectToAction(nameof(Register));

                }

                userDto.IsEmailVerified = false;
                userDto.EmailVerificationToken = _userAccountService.GenerateVerificationToken();

                var userAccountCreateDto = _userAccountService.Create(userDto);


                var verificationUrl = Url.Action("VerifyEmail", "UserAccount", new { token = userAccountCreateDto.EmailVerificationToken }, Request.Scheme);
                await _userAccountService.SendEmailVerificationAsync(userDto.Email, userDto.FirstName, verificationUrl);

                TempData["message"] = "Message";
                TempData["entity"] = _localizer["Please confirm your email before logging in."].ToString();

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
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var userAccount = await _userAccountService.GetUserByVerificationToken(token);

                if (userAccount != null)
                {
                    userAccount.IsEmailVerified = true;
                    userAccount.EmailVerificationToken = null;
                    _userAccountService.Update(userAccount);

                    TempData["message"] = "Success";
                    TempData["entity"] = "Email verified successfully! You can now log in.";
                }
                else
                {
                    TempData["message"] = "Error";
                    TempData["entity"] = "Invalid verification, Please contact our support Team.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "An error occurred while verifying the email. Please try again.";
            }

            return RedirectToAction(nameof(Login));
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

            if (!userDto.IsEmailVerified)
            {
                TempData["message"] = "Warning";
                TempData["entity"] = _localizer[$"We've sent an email verification to your registered email address ({userDto.Email}). Please check your inbox and follow the instructions to complete the email verification process before logging in."].ToString();
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

            //Represents the user
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });
        }

        [HttpGet]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var userAccount = await _userAccountService.GetByEmail(email);
                if (userAccount == null)
                {
                    TempData["message"] = "Error";
                    TempData["entity"] = _localizer["The provided email does not exist."].ToString();

                    return View();
                }

                var resetToken = _userAccountService.GeneratePasswordResetToken(userAccount.Email);

                var resetUrl = Url.Action("ResetPassword", "UserAccount", new { token = resetToken }, Request.Scheme);
                await _userAccountService.SendPasswordResetEmail(userAccount.Email, resetUrl);

                TempData["message"] = "Success";
                TempData["entity"] = "Password reset instructions have been sent to your email.";

                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the forgot password process.");

                TempData["message"] = "Error";
                TempData["entity"] = "An error occurred while processing your request. Please try again.";

                return View();
            }
        }

        [HttpGet]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["Invalid password reset token."].ToString();
                return RedirectToAction(nameof(Login));
            }

            var resetToken = new ResetPasswordDto { Token = token };
            return View(resetToken);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userAccount = await _userAccountService.GetUserByPasswordResetToken(resetPasswordDto.Token);

                    if (userAccount == null)
                    {
                        TempData["message"] = "Error";
                        TempData["entity"] = _localizer["Invalid Token!"].ToString();

                        return RedirectToAction(nameof(Login));
                    }

                    if (userAccount.PasswordResetTokenExpiry < DateTime.Now)
                    {
                        TempData["message"] = "Error";
                        TempData["entity"] = _localizer["Your token has expired."].ToString();

                        return RedirectToAction(nameof(Login));
                    }

                    _userAccountService.ResetPasword(userAccount, resetPasswordDto.Password);

                    _userAccountService.Update(userAccount);

                    TempData["message"] = "Success";
                    TempData["entity"] = "Password reset Successfully.";

                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the password reset process.");

                    TempData["message"] = "Error";
                    TempData["entity"] = "An error occurred while processing your request. Please try again.";

                    return RedirectToAction(nameof(Login));
                }

            }

            return RedirectToAction(nameof(Login));
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

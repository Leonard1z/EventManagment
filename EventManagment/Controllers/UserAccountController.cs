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
using System.Text.Json.Serialization;
using System.Text.Json;
using ReflectionIT.Mvc.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public UserAccountController(IUserAccountService userAccountService,
            IRoleService roleService,
            IEmailService emailService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<UserAccountController> localizer,
            ILogger<UserAccountController> logger,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration
            )
        {
            _userAccountService = userAccountService;
            _roleService = roleService;
            _emailService = emailService;
            _protector = provider.CreateProtector(purpose: purposeStrings.UserAccountControllerPs);
            _localizer = localizer;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpGet]
        [Route("UsersData")]
        public async Task<IActionResult> Index(string filter,string encryptedId,int pageSize=7,int page=1, string sortExpression = "Id")
        {
            try
            {
                var decryptedId = string.IsNullOrEmpty(encryptedId) ? null : _protector.Unprotect(encryptedId);

                var qry = _userAccountService.GetAllForPagination(filter, decryptedId);

                var dto = await PagingList.CreateAsync(qry, pageSize,page,sortExpression,"Name DESC");

                dto.RouteValue = new RouteValueDictionary { { "filter", filter }, { "pageSize", pageSize } };

                foreach (var user in dto)
                {
                    user.EncryptedId = _protector.Protect(user.Id.ToString());
                    user.Id = 0;
                }

                var totalCount = await qry.CountAsync();
                //get totalpages by dividing the totalcount and pagesize 20 / 7 = 2.857, then rounds it to (3)
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                ViewBag.Filter = filter;
                ViewBag.EncryptedId = encryptedId;
                ViewBag.PageSize = pageSize;
                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.SortExpression = sortExpression;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(dto);
                }
                else
                {
                    return View(dto);
                }


            }catch(Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString(); ;

                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<ActionResult> Login(string returnUrl)
        {
            if (Request.ContentType.ToString().Contains("application/json"))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var requestBody = await reader.ReadToEndAsync();
                    var loginDto = JsonConvert.DeserializeObject<LoginDto>(requestBody);
                    var userDto = _userAccountService.Authenticate(loginDto);

                    if (userDto == null)
                    {
                        return Unauthorized("Invalid Email or password");
                    }

                    var jwtToken = await GenerateJwtToken(userDto);

                    return Ok(new { Token = jwtToken });
                }

            }
            else
            {
                var loginDto = new LoginDto
                {
                    Email = Request.Form["Email"],
                    Password = Request.Form["Password"]
                };

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

        private async Task<string> GenerateJwtToken(UserAccountDto userDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = _configuration.GetSection("JwtSettings:SecretKey").Value;
            var key = Encoding.ASCII.GetBytes(jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                    new Claim(ClaimTypes.Name, userDto.FirstName),
                    new Claim(ClaimTypes.Email, userDto.Email),
                    new Claim(ClaimTypes.Role, userDto.Role.Name),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

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


        [HttpGet]
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

        [HttpGet]
        [Route("User/Edit")]
        public async Task<ActionResult> Edit(string encryptedId)
        {
            try
            {
                var id = int.Parse(_protector.Unprotect(encryptedId));
                var result = await _userAccountService.GetByIdEdit(id);

                UserEditDtoEncryption(result);

                return View(result);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [Route("User/Edit")]
        public ActionResult Edit(UserAccountEditDto userAccountEditDto)
        {
            try
            {
                userAccountEditDto.RoleId = userAccountEditDto.EncryptedRoleId != null ? int.Parse(_protector.Unprotect(userAccountEditDto.EncryptedRoleId)) : 0;
                userAccountEditDto.Id = int.Parse(_protector.Unprotect(userAccountEditDto.EncryptedId));

                var result = _userAccountService.UpdateWithRole(userAccountEditDto);

                TempData["message"] = "Updated";
                TempData["entity"] = _localizer["UserAccount "].ToString();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UserAccount/Profile")]
        public async Task<ActionResult> ProfileInformation()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value != null ? int.Parse(claim.Value) : 0;

            var user = await _userAccountService.GetProfileById(userId);

            user.EncryptedId = _protector.Protect(user.Id.ToString());
            user.EncryptedRoleId = _protector.Protect(user.RoleId.ToString());
            user.Id = 0;
            user.RoleId = 0;

            return View(user);
        }      

        [Authorize]
        [HttpPost]
        [Route("UserAccount/UpdateUserProfile")]
        public async Task<IActionResult> SubmitProfileUpdate(ProfileUpdateDto profileUpdateDto, IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                    if (allowedExtensions.Contains(extension))
                    {
                        string uploads = Path.Combine(wwwRootPath, @"images\profile");
                        string filePath = Path.Combine(uploads, fileName + extension);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        profileUpdateDto.ProfileImage = @"\images\profile\" + fileName + extension;
                    }
                    else
                    {
                        TempData["message"] = "Error";
                        TempData["entity"] = "Invalid file extension. Allowed extensions are .jpg, .jpeg, and .png.";

                        return RedirectToAction("Index", "Home");
                    }
                }

                profileUpdateDto.Id = profileUpdateDto.EncryptedId != null ? int.Parse(_protector.Unprotect(profileUpdateDto.EncryptedId)) : 0;
                profileUpdateDto.RoleId = profileUpdateDto.EncryptedRoleId != null ? int.Parse(_protector.Unprotect(profileUpdateDto.EncryptedRoleId)) : 0;

                var role = await _roleService.GetById(profileUpdateDto.RoleId);

                _userAccountService.UpdateUserProfile(profileUpdateDto);

                TempData["message"] = "Success";
                TempData["entity"] = "Profile updated successfully.";

                return RedirectToAction("Index", "Home");

            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "An error occurred while updating the profile. Please try again.";

                return RedirectToAction("Index","Home");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UserAccount/ChangePassword")]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordDto());
        }
        [Authorize]
        [HttpPost]
        [Route("UserAccount/ChangePassword")]
        public async Task<IActionResult> SubmitPasswordChange(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim.Value != null ? int.Parse(claim.Value) : 0;

                var user = await _userAccountService.GetProfileById(userId);

                if (!_userAccountService.VerifyPassword(changePasswordDto.OldPassword, user.Password, user.Salt))
                {
                    ModelState.AddModelError("OldPassword", "Incorrect old password.");
                    return View("ChangePassword", changePasswordDto);
                }

                await _userAccountService.ChangePassword(user.Id, changePasswordDto.NewPassword);

                TempData["message"] = "Success";
                TempData["entity"] = "Password changed successfully.";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "An error occurred while changing the password. Please try again.";

                return RedirectToAction("Index", "Home");
            }
        }
        private UserAccountEditDto UserEditDtoEncryption(UserAccountEditDto userAccountEdit)
        {
            userAccountEdit.Role = _roleService.GetAll().Result.Select(x =>
            {
                x.EncryptedId = _protector.Protect(x.Id.ToString());
                if (userAccountEdit.RoleId == x.Id) userAccountEdit.EncryptedRoleId = x.EncryptedId;
                x.Id = 0;

                return x;
            }).ToList();
            userAccountEdit.EncryptedId = _protector.Protect(userAccountEdit.Id.ToString());
            userAccountEdit.Id = 0;

            return userAccountEdit;
        }
    }
}

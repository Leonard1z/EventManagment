using Domain._DTO.Verification;
using Domain.Entities;
using EventManagment.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Services.Notification;
using Services.Role;
using Services.Twilio;
using Services.UserAccount;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;

namespace EventManagment.ApiControllers
{
    [Route("api/verification")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly TwilioService _twilioService;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<VerificationController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserAccountService _userAccountService;
        private readonly IRoleService _roleService;
        private readonly INotificationService _notificationService;

        public VerificationController(IDistributedCache distributedCache,
            ILogger<VerificationController> logger,
            IHubContext<NotificationHub> hubContext,
            IUserAccountService userAccountService,
            IRoleService roleService,
            INotificationService notificationService
            )
        {
            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var twilioPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

            _twilioService = new TwilioService(accountSid, authToken, twilioPhoneNumber);

            _distributedCache = distributedCache;
            _logger = logger;
            _hubContext = hubContext;
            _userAccountService = userAccountService;
            _roleService = roleService;
            _notificationService = notificationService;
        }

        [HttpPost("send-code")]
        public IActionResult SendVerificationCode([FromBody] VerificationRequestDto request)
        {
            string verificationCode = GenerateRandomCode();

            SaveVerificationCode(request.PhoneNumber, verificationCode);

            _twilioService.SendSms(request.PhoneNumber, $"Your verification code is: {verificationCode}");

            var verificationPageUrl = Url.Action("VerifyCode", "Verification", new { phoneNumber = request.PhoneNumber }, Request.Scheme);

            return Ok(new { Success=true, VerificationPageUrl = verificationPageUrl, Message = "Verification code sent successfully" });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyVerificationCode([FromBody] VerificationRequestDto request)
        {
            string savedCode = await GetSavedVerificationCode(request.PhoneNumber);

            if (request.VerificationCode == savedCode)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _userAccountService.GetById(userId);
                var role = await _roleService.GetRoleByName("EventCreator");
                if (user != null && role !=null)
                {
                    user.PhoneNumber = request.PhoneNumber;
                    user.RoleId = role.Id;
                    await _userAccountService.UpdateAsync(user);
                    var adminMessage = $"User with ID {userId} has updated their role";
                    try
                    {
                        await _notificationService.Create(new Domain.Entities.Notification
                        {
                            Message = adminMessage,
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            Type = "AdminNotification"
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", "user updated their role");
                }

                return Ok(new { Success=true, PhoneNumber = request.PhoneNumber, Message = "Verification code is valid" });
            }

            return BadRequest(new { Message = "Invalid verification code" });
        }

        private string GenerateRandomCode()
        {
            int codeLength = 6;

            string validChars = "0123456789";

            Random random = new Random();
            StringBuilder codeBuilder = new StringBuilder();

            for (int i = 0; i < codeLength; i++)
            {
                int index = random.Next(validChars.Length);
                codeBuilder.Append(validChars[index]);
            }

            return codeBuilder.ToString();
        }

        private void SaveVerificationCode(string phoneNumber, string verificationCode)
        {

            var cacheKey = $"VerificationCode_{phoneNumber}";

            _distributedCache.SetString(cacheKey, verificationCode,new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(5)
            });
        }

        private async Task<string> GetSavedVerificationCode(string phoneNumber)
        {
            var cacheKey = $"VerificationCode_{phoneNumber}";
            var savedCode = await _distributedCache.GetStringAsync(cacheKey);

            return savedCode;
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security;
using Services.Events;
using Services.Registration;
using System.Security.Claims;

namespace EventManagment.ApiControllers
{
    [Route("api/event")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventApiController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<EventApiController> _logger;
        private readonly IDataProtector _protector;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EventApiController(
            IRegistrationService registrationService,
            IEventService eventService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            ILogger<EventApiController> logger,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _registrationService = registrationService;
            _eventService = eventService;
            _protector = provider.CreateProtector(purpose: purposeStrings.ApiControllersPs);
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            try
            {
                var userId = GetUserIdFromToken();

                if(userId == 0)
                {
                    return BadRequest("Invalid User.");
                }

                var upcomingEvents = await _eventService.GetUpcomingEventsWithinOneWeek(userId);



                foreach(var item in upcomingEvents)
                {
                    item.EncryptedId = _protector.Protect(item.Id.ToString());
                    item.EncryptedCategoryId = _protector.Protect(item.CategoryId.ToString());
                    item.EncryptedUserAccountId = _protector.Protect(item.UserAccountId.ToString());
                    item.Id = 0;
                    item.CategoryId = 0;
                    item.UserAccountId = 0;
                }

                return Ok(upcomingEvents);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching upcoming events");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("assignedTickets/{encryptedId}")]
        public async Task<IActionResult> GetAssignedTicketsForEvent(string encryptedId)
        {

            var eventId = encryptedId != null ? int.Parse(DecryptId(encryptedId)) : 0;
            if (eventId == 0)
            {
                return BadRequest("Invalid or decrypted ID.");
            }
            var assignedTickets = await _registrationService.GetAssignedTicketsForEvent(eventId);

            foreach(var ticket in assignedTickets)
            {
                ticket.EncryptedId = _protector.Protect(ticket.Id.ToString());
                ticket.EncryptedRegistrationId = _protector.Protect(ticket.RegistrationId.ToString());
                ticket.Id = 0;
                ticket.RegistrationId = 0;
            }
            return Ok(assignedTickets);
        }
        private int GetUserIdFromToken()
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal?.Identity != null && principal.Identity.IsAuthenticated)
            {
                var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
            }

            throw new InvalidOperationException("Unable to retrieve user ID from the authentication token.");
        }
        private string DecryptId(string encryptedId)
        {
            try
            {
                var decryptedId = _protector.Unprotect(encryptedId);
                return decryptedId;

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error decrypting ID");
                return null;
            }
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Security;
using Services.Events;
using System.Security.Claims;

namespace EventManagment.ApiControllers
{
    [Route("api/event")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventApiController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventApiController> _logger;
        private readonly IDataProtector _protector;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EventApiController(
            IEventService eventService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            ILogger<EventApiController> logger,
            IHttpContextAccessor httpContextAccessor
            )
        {
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
    }
}

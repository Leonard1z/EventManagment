using Domain._DTO.Event;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Security;
using Services.Events;
using Services.Registration;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class RegistrationController : Controller
    {

        private readonly IRegistrationService _registrationService;
        private readonly IEventService _eventService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<RegistrationController> _localizer;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IRegistrationService registrationService,
            IEventService eventService,
             IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<RegistrationController> localizer,
            ILogger<RegistrationController> logger
            )
        {
            _registrationService = registrationService;
            _eventService = eventService;
            _protector = provider.CreateProtector(purpose: purposeStrings.HomeControllerPs);
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IActionResult> RegisterToEvent(string encryptedEventId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "UserAccount");
                }


                var eventId = int.Parse(_protector.Unprotect(encryptedEventId));

                var events = await _eventService.GetById(eventId);

                if (events == null)
                {
                    return NotFound();
                }

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim.Value != null ? int.Parse(claim.Value) : 0;

                bool isRegistered = await _registrationService.CheckIfUserExist(userId, eventId);

                if (isRegistered)
                {
                    TempData["message"] = "Error";
                    TempData["entity"] = "You are already registered for this event.";
                    return RedirectToAction("Index", "Home");
                }

                await _registrationService.RegisterUserForEvent(userId, eventId);

                TempData["message"] = "Success";
                TempData["entity"] = "You have been registered successfully";

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();


                return RedirectToAction("Index", "Home");
            }
        }
    }
}

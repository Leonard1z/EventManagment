using Domain.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Security;
using Services.Events;
using Services.Registration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EventManagment.Controllers
{
    [Authorize(Policy = "AccessDashboard")]
    public class DashboardController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<DashboardController> _localizer;

        public DashboardController(IEventService eventService,
            IRegistrationService registrationService,
            ILogger<DashboardController> logger,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<DashboardController> localizer
            )
        {
            _eventService = eventService;
            _registrationService = registrationService;
            _logger = logger;
            _protector = provider.CreateProtector(purpose: purposeStrings.DashboardControllerPs);
            _localizer = localizer;
        }

        [Route("Dashboard")]
        [Authorize(Policy = "ViewDashboardData")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetAllData")]
        public async Task<IActionResult> GetAllData()
        {

            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(claim.Value);
                var isAdmin = User.IsInRole("Admin");

                var dashboardData = new DashboardViewModel();

                if (isAdmin)
                {
                    dashboardData.TotalEventsCreated = await _eventService.GetTotalEventCount();
                    dashboardData.TotalTicketsSold = await _registrationService.GetTotalTicketsSoldForAdmin();
                    dashboardData.TotalUpcomingEvents = await _eventService.GetTotalUpcomingEventsForAdmin();

                }
                else
                {
                    dashboardData.TotalEventsCreated = await _eventService.GetTotalEventCountForEventCreator(userId);
                    dashboardData.TotalTicketsSold = await _registrationService.GetTotalTicketsSoldForUser(userId);
                    dashboardData.TotalUpcomingEvents = await _eventService.GetTotalUpcomingEventsForEventCreator(userId);
                }

                return Json(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "Invalid value.";

                return RedirectToAction(nameof(Index));
            }
        }
        [Route("UpcomingEvents")]
        public async Task<IActionResult> UpcomingEvents()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(claim.Value);
                var isAdmin = User.IsInRole("Admin");

                if (isAdmin)
                {
                    var upcomingEvents = await _eventService.GetUpcomingEventsForAdmin();

                    return View(upcomingEvents);
                }
                else
                {
                    var upcomingEvents = await _eventService.GetUpcomingEvents(userId);

                     return View(upcomingEvents);
                }

                return RedirectToAction(nameof(Index));

            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "Invalid value.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}

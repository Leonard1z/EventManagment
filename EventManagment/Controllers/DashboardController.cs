using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Security;
using Services.Events;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<DashboardController> _localizer;

        public DashboardController(IEventService eventService,
            ILogger<DashboardController> logger,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<DashboardController> localizer
            )
        {
            _eventService = eventService;
            _logger = logger;
            _protector = provider.CreateProtector(purpose: purposeStrings.DashboardControllerPs);
            _localizer = localizer;
        }
        [Route("Dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetEventCountForDashboard")]
        public async Task<IActionResult> GetEventCountForDashboard()
        {

            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(claim.Value);
                var isAdmin = User.IsInRole("Admin");
                int eventCount;
                if (isAdmin)
                {
                    eventCount = await _eventService.GetTotalEventCount();
                }
                else
                {
                    eventCount = await _eventService.GetTotalEventCountForEventCreator(userId);
                }

                return Json(new { eventCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                TempData["message"] = "Error";
                TempData["entity"] = "Invalid value.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}

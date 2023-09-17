using Microsoft.AspNetCore.Mvc;
using EventManagment.Models;
using Services.Events;
using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Localization;
using Security;

namespace EventManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILogger<HomeController> _logger;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IEventService eventService,
            ILogger<HomeController> logger,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<HomeController> localizer
            )
        {
            _eventService = eventService;
            _logger = logger;
            _protector = provider.CreateProtector(purpose: purposeStrings.HomeControllerPs);
            _localizer = localizer;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var events = await _eventService.GetAllByIsActive();

                foreach (var item in events)
                {
                    item.EncryptedId = _protector.Protect(item.Id.ToString());
                    item.Id = 0;
                }

                return View(events);

            }
            catch (Exception ex)
            {
                TempData["message"] = "Error";
                TempData["entity"] = _localizer["An error occurred, try again"].ToString();

                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace EventManagment.Controllers
{
    public class DashboardController : Controller
    {
        [Route("Dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

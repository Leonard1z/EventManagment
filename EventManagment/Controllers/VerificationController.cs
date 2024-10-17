using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagment.Controllers
{
    public class VerificationController : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("Verification/VerifyPhoneNumber")]
        public IActionResult VerifyPhoneNumber()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        [Route("Verification/VerifyCode")]
        public IActionResult VerifyCode(string phoneNumber)
        {
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }
    }
}

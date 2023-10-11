using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace EventManagment.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IDistributedCache _cache;

        public PaymentController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        [Route("Payment")]
        public IActionResult Payment(string token)
        {
            if (!IsTokenValid(token))
            {
                return RedirectToAction("TokenInvalid");
            }


            RemoveToken(token);

            return View("PaymentSuccess");
        }

        public IActionResult TokenInvalid()
        {
            return View();
        }

        private bool IsTokenValid(string token)
        {
            return !string.IsNullOrEmpty(token) && CheckTokenInCache(token);
        }

        private bool CheckTokenInCache(string token)
        {
            byte[] cachedToken = _cache.Get(token);
            return cachedToken != null && cachedToken.Length > 0;
        }


        private void RemoveToken(string token)
        {
            _cache.Remove(token);
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Services.Reservation;
using System.Text;
using System.Text.Json;

namespace EventManagment.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IDistributedCache _cache;
        private readonly IReservationService _reservationService;

        public PaymentController(IDistributedCache cache,IReservationService reservationService)
        {
            _cache = cache;
            _reservationService = reservationService;
        }

        [HttpGet]
        [Route("Payment")]
        public async Task<IActionResult> Payment(string token)
        {
            if (!IsTokenValid(token))
            {
                return RedirectToAction("TokenInvalid");
            }

            var reservationID = await GetReservationIdFromToken(token);

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

        public async Task<int> GetReservationIdFromToken(string paymentToken)
        {

            var serializedInformation = await _cache.GetStringAsync(paymentToken);

            if (serializedInformation != null)
            {
                var storedInformation = JsonSerializer.Deserialize<object>(serializedInformation);

                if (storedInformation is JsonElement jsonElement && jsonElement.TryGetProperty("ReservationId", out var reservationIdElement))
                {
                    return reservationIdElement.GetInt32();
                }
            }

            return 0;

        }

        private void RemoveToken(string token)
        {
            _cache.Remove(token);
        }
    }
}


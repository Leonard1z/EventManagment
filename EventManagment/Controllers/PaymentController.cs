using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Services.Reservation;
using Stripe;
using Stripe.Checkout;
using System.Text;
using System.Text.Json;

namespace EventManagment.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IDistributedCache _cache;
        private readonly IReservationService _reservationService;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IDistributedCache cache, IReservationService reservationService, IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentController> logger
            )
        {
            _cache = cache;
            _reservationService = reservationService;
            _stripeSettings = stripeSettings.Value;
            _logger = logger;

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
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
            var reservation = await _reservationService.GetByIdWithTicket(reservationID);
            await _reservationService.UpdateReservationStatus(reservationID, ReservationStatus.PaymentInProgress);

            var domain = "https://localhost:44331/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"success?rt={token}",
                CancelUrl = domain + $"index",
            };

            TempData["PaymentToken"] = token;

            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(reservation.TicketTypes.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = reservation.TicketTypes.Name,
                    },
                },
                Quantity = reservation.Quantity,
            };
            options.LineItems.Add(sessionLineItem);
            try
            {
                var service = new SessionService();
                Session session = service.Create(options);
                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("PaymentError");
            }
        }
        public IActionResult TokenInvalid()
        {
            return View();
        }

        [HttpGet]
        [Route("success")]
        public async Task<IActionResult> Success()
        {
            var token = TempData["PaymentToken"] as string;

            if (!string.IsNullOrEmpty(token))
            {
                var reservationId = await GetReservationIdFromToken(token);
                await _reservationService.UpdateReservationStatus(reservationId, ReservationStatus.Paid);
                RemoveToken(token);

                return View();
            }

            return RedirectToAction("TokenNotFound");
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


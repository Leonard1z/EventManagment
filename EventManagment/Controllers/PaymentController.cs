using Domain.Entities;
using EventManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Registration;
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
        private readonly IRegistrationService _registrationService;
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IDistributedCache cache, IReservationService reservationService,
            IRegistrationService registrationService,
            IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentController> logger
            )
        {
            _cache = cache;
            _reservationService = reservationService;
            _registrationService = registrationService;
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

            bool isUserRegistered = await _registrationService.IsUserRegisteredAsync(reservation.UserAccountId, reservation.TicketTypes.EventId, reservation.TicketTypeId);

            if (isUserRegistered)
            {
                TempData["message"] = "Error";
                TempData["entity"] = "You are already registered for this event.";
                RemoveToken(token);
                return RedirectToAction("Index", "Home");
            }


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
                SuccessUrl = domain + $"success?rt={token}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = domain + $"index",
                ClientReferenceId = reservation.UserAccountId.ToString(),
                Metadata = new Dictionary<string, string>
                {
                    { "ticketId", reservation.TicketTypeId.ToString() },
                    { "eventId", reservation.TicketTypes.EventId.ToString()},
                    { "quantity", reservation.Quantity.ToString()},
                    { "ticketPrice", reservation.TicketTypes.Price.ToString() },
                },
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
        [Route("TokenInvalid")]
        public IActionResult TokenInvalid()
        {
            return View("Error", new ErrorViewModel { StatusCode = 400, ErrorMessage = "The provided token is invalid. Please try again." });
        }
        [Route("PaymentError")]
        public IActionResult PaymentError()
        {
            return View("Error", new ErrorViewModel { StatusCode = 500, ErrorMessage = "An error occurred during the payment process. Please try again." });
        }

        [HttpGet]
        [Route("success")]
        public async Task<IActionResult> Success()
        {
            var token = TempData["PaymentToken"] as string;
            var sessionID = HttpContext.Request.Query["session_id"].ToString();

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(sessionID))
            {
                var reservationId = await GetReservationIdFromToken(token);
                await _reservationService.UpdateReservationStatus(reservationId, ReservationStatus.Paid);

                var service = new SessionService();
                var session = service.Get(sessionID);

                var quantity = Convert.ToInt32(session.Metadata["quantity"]);
                var ticketPrice = Convert.ToInt32(session.Metadata["ticketPrice"]);
                var totalPrice = quantity * ticketPrice;
                var paymentIntentId = session.PaymentIntentId;

                var registration = new Registration
                {
                    RegistrationDate = DateTime.Now,
                    TransactionId = paymentIntentId,
                    Quantity = quantity,
                    TicketPrice = ticketPrice,
                    TotalPrice = totalPrice,
                    UserAccountId = Convert.ToInt32(session.ClientReferenceId),
                    EventId = Convert.ToInt32(session.Metadata["eventId"]),
                    TicketTypeId = Convert.ToInt32(session.Metadata["ticketId"])
                };

                await _registrationService.RegisterUserForEventAsync(registration);

                RemoveToken(token);

                return View();
            }

            return RedirectToAction("TokenNotFound");
        }
        [Route("TokenNotFound")]
        public IActionResult TokenNotFound()
        {
            return View("Error",new ErrorViewModel { StatusCode = 404, ErrorMessage = "An unexpected error occurred. Please contact support for assistance." });
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


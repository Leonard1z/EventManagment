using Domain.Entities;
using Domain.ViewModels;
using EventManagment.Hubs;
using Infrastructure.Repositories.Notifications;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Security;
using Services.Notification;
using Services.Reservation;
using Services.Tickets;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class ReservationController : Controller
    {

        private readonly IReservationService _reservationService;
        private readonly ITicketTypeService _ticketTypeService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<ReservationController> _localizer;
        private readonly ILogger<ReservationController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReservationController(IReservationService reservationService,
            ITicketTypeService ticketTypeService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<ReservationController> localizer,
            ILogger<ReservationController> logger,
            IHubContext<NotificationHub> hubContext
            )
        { 
            _reservationService = reservationService;
            _ticketTypeService = ticketTypeService;
            _protector = provider.CreateProtector(purpose: purposeStrings.ReservationControllerPs);
            _localizer = localizer;
            _logger = logger;
            _hubContext = hubContext;
        }


        [HttpPost]
        [Route("Reservation/Reserve")]
        public async Task<ActionResult> Reserve([FromBody] ReservationRequest request)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userId = int.Parse(claim);

                var hasActiveReservation = await _reservationService.HasActiveReservationForTickets(userId, new List<int> { request.TicketId });

                if (hasActiveReservation)
                {
                    return Json(new { success = false, message = "You already have an active reservation for this ticket." });
                }

                var hasCompletedPayment = await _reservationService.HasCompletedPayment(userId, request.EventId, request.TicketId);

                if (hasCompletedPayment)
                {
                    return Json(new { success = false, message = "You have already completed payment for this ticket." });
                }

                if (request.Quantity <= 0)
                {
                    return BadRequest(new { Message = "Quantity must be greater than 0." });
                }

                var maxAllowedQuantity = 7;
                if (request.Quantity > maxAllowedQuantity)
                {
                    return Json(new { success = false, message = $"You cannot exceed the limit of {maxAllowedQuantity} tickets." });
                }

                var ticket = await _ticketTypeService.GetTicketByIdAsync(request.TicketId);

                if(ticket == null)
                {
                    return NotFound(new { success = false, Message = "Ticket Not Found" });
                }  

                if(ticket.Quantity < request.Quantity)
                {
                    return BadRequest(new { success = false, Message = "Not enought tickets available" });
                }

                await _reservationService.Create(request.TicketId,request.EventId, userId, request.Quantity, request.TicketTotalPrice);

                await _hubContext.Clients.User(userId.ToString()).SendAsync("UpdateNotificationCountAndData");

                return Ok(new { success = true, Message = "Reservation made successful. Please check your email or your notification in Home Page" });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error ocurred during reservation");

                return StatusCode(500, new { Message = "An error occurred during reservation. Please try again" });
            }
        }
    }
}

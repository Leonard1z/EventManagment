using Domain.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Security;
using Services.Events;
using Services.Registration;
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

        public ReservationController(IReservationService reservationService,
            ITicketTypeService ticketTypeService,
             IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<ReservationController> localizer,
            ILogger<ReservationController> logger
            )
        { 
            _reservationService = reservationService;
            _ticketTypeService = ticketTypeService;
            _protector = provider.CreateProtector(purpose: purposeStrings.ReservationControllerPs);
            _localizer = localizer;
            _logger = logger;
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

                var ticket = await _ticketTypeService.GetTicketByIdAsync(request.TicketId);

                if(ticket == null)
                {
                    return NotFound(new { success = false, Message = "Ticket Not Found" });
                }  

                if(ticket.Quantity < request.Quantity)
                {
                    return BadRequest(new { success = false, Message = "Not enought tickets available" });
                }

                await _reservationService.Create(request.TicketId, userId, request.Quantity);

                return Ok(new { success = true, Message = "Reservation successful" });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error ocurred during reservation");

                return StatusCode(500, new { Message = "An error occurred during reservation" });
            }
        }
    }



}

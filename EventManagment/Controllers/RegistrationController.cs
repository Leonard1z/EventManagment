using Domain._DTO.Event;
using Domain.Entities;
using Domain.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Security;
using Services.Events;
using Services.Registration;
using Services.Tickets;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class RegistrationController : Controller
    {

        private readonly IRegistrationService _registrationService;
        private readonly IEventService _eventService;
        private readonly ITicketTypeService _ticketTypeService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<RegistrationController> _localizer;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IRegistrationService registrationService,
            IEventService eventService,
            ITicketTypeService ticketTypeService,
             IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<RegistrationController> localizer,
            ILogger<RegistrationController> logger
            )
        {
            _registrationService = registrationService;
            _eventService = eventService;
            _ticketTypeService = ticketTypeService;
            _protector = provider.CreateProtector(purpose: purposeStrings.HomeControllerPs);
            _localizer = localizer;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        [Route("Registration/RegisterForFreeTickets")]
        public async Task<IActionResult> RegisterForFreeTickets([FromBody]FreeTicketsRegistrationViewModel request)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim.Value != null ? int.Parse(claim.Value) : 0;

                bool isUserRegistered = await _registrationService.IsUserRegisteredAsync(userId, request.EventId, request.TicketId);
                if (isUserRegistered)
                {
                    return Json(new {success = false, Message = "You are already registered for this event." });
                }

                if (request.Quantity <= 0)
                {
                    return Json(new { success = false, Message = "Quantity must be greater than 0." });
                }
                var maxAllowedQuantity = 7;
                if (request.Quantity > maxAllowedQuantity)
                {
                    return Json(new { success = false, message = $"You cannot exceed the limit of {maxAllowedQuantity} tickets." });
                }
                var ticket = await _ticketTypeService.GetTicketByIdAsync(request.TicketId);
                if (ticket == null)
                {
                    return Json(new { success = false, Message = "Ticket Not Found" });
                }

                if (ticket.Quantity < request.Quantity)
                {
                    return Json(new { success = false, Message = "Not enought tickets available" });
                }
                var registration = new Registration
                {
                    RegistrationDate = DateTime.Now,
                    IsAssigned = false,
                    Quantity = request.Quantity,
                    UserAccountId = userId,
                    EventId=request.EventId,
                    TicketTypeId = request.TicketId
                };

                ticket.Quantity -= request.Quantity;
                await _ticketTypeService.UpdateAsync(ticket);
                await _registrationService.RegisterUserForEventAsync(registration);
                return Ok(new { success = true, Message = "You have been registered successfully, Please check your ticket page." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred during reservation");

                return StatusCode(500, new { Message = "An error occurred during reservation. Please try again" });
            }
        }
    }
}

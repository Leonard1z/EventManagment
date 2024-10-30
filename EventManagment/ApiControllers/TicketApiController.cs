using Domain._DTO.Ticket;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Security;
using Services.AssigneTicket;
using Services.Events;
using Services.Tickets;
using System.Security.Claims;

namespace EventManagment.ApiControllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
    public class TicketApiController : ControllerBase
    {
        private readonly IAssigneTicketService _assigneTicketService;
        private readonly ITicketTypeService _ticketTypeService;
        private readonly IEventService _eventService;
        private readonly ILogger<TicketApiController> _logger;
        private readonly IDataProtectionProvider _provider;
        private readonly DataProtectionPurposeStrings _purposeStrings;
        public TicketApiController(IAssigneTicketService assigneTicketService,
            ITicketTypeService ticketTypeService,
            IEventService eventService,
            ILogger<TicketApiController> logger,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings
            )
        {
            _assigneTicketService = assigneTicketService;
            _ticketTypeService = ticketTypeService;
            _eventService = eventService;
            _logger = logger;
            _provider = provider;
            _purposeStrings = purposeStrings;
        }
        private IDataProtector CreateProtector(string purposeString)
        {
            return _provider.CreateProtector(purpose:purposeString);
        }
        [HttpPost("UpdateIsInsideEvent")]
        public async Task<IActionResult> UpdateIsInsideEvent([FromBody] ScannedData scannedData)
        {
            try
            {
                var ticketToUpdate = await _assigneTicketService.GetTicketByTicketNumberAsync(scannedData.TicketNumber);

                if (ticketToUpdate != null)
                {
                    //if the IsInsideEvent is true make it false and around
                    ticketToUpdate.IsInsideEvent = !ticketToUpdate.IsInsideEvent;

                    await _assigneTicketService.UpdateAsync(ticketToUpdate);

                    return Ok(new { Message = "IsInsideEvent updated successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Ticket not found" });
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating IsInsideEvent");
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        [HttpDelete]
        [Route("DeleteTicket")]
        public ActionResult DeleteTicket(string encryptedId)
        {
            try
            {
                var protector = CreateProtector(_purposeStrings.EventControllerPs);
                var id = int.Parse(protector.Unprotect(encryptedId.ToString()));

                var hasRegistrations = _ticketTypeService.HasRegistrations(id);
                var hasReservations = _ticketTypeService.HasReservations(id);

                if (hasRegistrations || hasReservations)
                {
                    return BadRequest(new { success = false, message = "Cannot delete ticket due to existing registrations or reservations." });
                }

                var result = _ticketTypeService.Delete(id);

                return Ok(new { success = true, message = "Ticket deleted successfully" });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, "Error deleting ticket");

                return StatusCode(500, "An error occurred while processing the request");
            }

        }
        [HttpGet]
        [Route("UpdateTicket")]
        public async Task<ActionResult> EditTicket(string encryptedId)
        {
            try
            {
                var protector = CreateProtector(_purposeStrings.EventControllerPs);
                var id = int.Parse(protector.Unprotect(encryptedId.ToString()));

                var ticket = await _ticketTypeService.GetById(id);

                ticket.EncryptedId = protector.Protect(id.ToString());
                ticket.EncryptedEventId = protector.Protect(ticket.EventId.ToString());
                ticket.Id = 0;
                ticket.EventId = 0;

                if (ticket == null)
                {
                    return NotFound("Ticket not found.");
                }

                return Ok(new { success = true, data = ticket });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("EditTicket")]
        public async Task<IActionResult> EditTicket([FromBody]TicketTypeEditDto formData)
        {
            try
            {

                var protector = CreateProtector(_purposeStrings.EventControllerPs);
                formData.Id = int.Parse(protector.Unprotect(formData.EncryptedId.ToString()));
                formData.EventId = int.Parse(protector.Unprotect(formData.EncryptedEventId.ToString()));
                var result = await _eventService.GetById(formData.EventId);
                var ticketStartDate = formData.SaleStartDate;
                var ticketEndDate = formData.SaleEndDate;

                if (ticketStartDate>result.EndDate || ticketEndDate>result.EndDate || ticketStartDate > ticketEndDate)
                {
                    return BadRequest(new { Message = "Ticket dates must be within the event's timeframe." });
                }
                if (!formData.IsFree)
                {
                    if (formData.Price <= 0)
                    {
                        ModelState.AddModelError("Price", "Price must be a positive number.");
                        return BadRequest(ModelState);
                    }
                }
                if (formData.Quantity <= 0)
                {
                    ModelState.AddModelError("Quantity", "Quantity must be a positive number.");
                    return BadRequest(ModelState);
                }
                await _ticketTypeService.UpdateAsync(formData);

                return Ok(new { success = true, Message = "Ticket updated successfully." });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);

                return StatusCode(500, $"An error occurred while adding the ticket: {ex.Message}");
            }
        }
    }
}
  
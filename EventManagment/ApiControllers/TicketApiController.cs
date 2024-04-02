using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Security;
using Services.AssigneTicket;
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
        ITicketTypeService _ticketTypeService;
        private readonly ILogger<TicketApiController> _logger;
        private readonly IDataProtectionProvider _provider;
        private readonly DataProtectionPurposeStrings _purposeStrings;
        public TicketApiController(IAssigneTicketService assigneTicketService,
            ITicketTypeService ticketTypeService,
            ILogger<TicketApiController> logger,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings
            )
        {
            _assigneTicketService = assigneTicketService;
            _ticketTypeService = ticketTypeService;
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
                    return BadRequest(new { success = false, message = "Cannot delete ticket with registrations or reservations." });
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
    }
}
  
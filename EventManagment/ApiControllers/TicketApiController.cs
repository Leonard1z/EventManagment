using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AssigneTicket;

namespace EventManagment.ApiControllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class TicketApiController : ControllerBase
    {
        private readonly IAssigneTicketService _assigneTicketService;
        private readonly ILogger<TicketApiController> _logger;
        public TicketApiController(IAssigneTicketService assigneTicketService, ILogger<TicketApiController> logger)
        {
            _assigneTicketService = assigneTicketService;
            _logger = logger;
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

    }
}
  
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.AssigneTicket;
using Services.Registration;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class PurchasedTickets : Controller
    {
        private readonly IAssigneTicketService _assigneTicketService;
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<PurchasedTickets> _logger;
        public PurchasedTickets(IAssigneTicketService assigneTicketService,
            IRegistrationService registrationService,
            ILogger<PurchasedTickets> logger)
        {
            _assigneTicketService= assigneTicketService;
            _registrationService= registrationService;
            _logger = logger;
        }
        public async Task <IActionResult> Index()
        {
            try
            {
                var userClaim = (ClaimsIdentity)User.Identity;
                var user = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var userId = user.Value != null ? int.Parse(user.Value) : 0;

                var purchasedTickets = await _registrationService.GetUserPurchasedTicketsAsync(userId);

                return View(purchasedTickets);

            }catch(Exception ex)
            {
                _logger.LogError(ex.Message,"An error occurred while fetching user purchased tickets.");

                TempData["message"] = "Error";
                TempData["entity"] = "An Error occurred, Please Try Again";

                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [Route("Assign/Ticket")]
        public async Task<IActionResult> AssignTickets([FromBody] List<AssignedTicket> assigneeData)
        {
            try
            {

                var result = await _assigneTicketService.AssignTicketsAsync(assigneeData);

                if (result)
                {
                    return Ok(new { Success=true, Message = "Tickets assigned successfully!" });
                }
                else
                {
                    return StatusCode(500, "Internal Server Error");
                }

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error assigning ticket");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        [Route("/GetRegistration")]
        public async Task<IActionResult> GetRegistration(int registrationId)
        {
            try
            {
                var registration = await _registrationService.GetRegistrationById(registrationId);

                if (registration != null)
                {
                    return Ok(registration);
                }
                else
                {
                    return NotFound("Registration not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}

using Microsoft.AspNetCore.Mvc;

namespace EventManagment.Controllers
{
    public class ReservationController : Controller
    {
        [HttpPost]
        [Route("Reservation/Reserve")]
        public IActionResult Reserve([FromBody] ReservationRequest request)
        {
            try
            {

                return Ok(new { Message = "Reservation successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during reservation" });
            }
        }
    }

    public class ReservationRequest
    {
        public int TicketId { get; set; }
        public int Quantity { get; set; }
    }

}

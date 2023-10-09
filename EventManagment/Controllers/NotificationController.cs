using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Notification;
using System.Security.Claims;

namespace EventManagment.Controllers
{
    public class NotificationController : Controller
    {

        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        [Route("Notification/GetNotifications")]
        public async Task<ActionResult> GetNotificationCount()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    var userId = int.Parse(claim.Value);

                    var notificationsData = await _notificationService.GetNotificationDataByUserId(userId);
                    var notificationCount = await _notificationService.GetUnreadNotificationCountByUserId(userId);

                    return Ok( new {data = notificationsData, count = notificationCount} );
                }
                else
                {
                    return Unauthorized(new { Message = "User is not authenticated" });
                }

            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching notification count" });

            }
        }
    }
}

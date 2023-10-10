﻿using Domain.Entities;
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
        private readonly INotificationService _notificationService;
        private readonly IDataProtector _protector;
        private readonly IStringLocalizer<ReservationController> _localizer;
        private readonly ILogger<ReservationController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReservationController(IReservationService reservationService,
            ITicketTypeService ticketTypeService,
            INotificationService notificationService,
            IDataProtectionProvider provider,
            DataProtectionPurposeStrings purposeStrings,
            IStringLocalizer<ReservationController> localizer,
            ILogger<ReservationController> logger,
            IHubContext<NotificationHub> hubContext
            )
        { 
            _reservationService = reservationService;
            _ticketTypeService = ticketTypeService;
            _notificationService = notificationService;
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

                var ticket = await _ticketTypeService.GetTicketByIdAsync(request.TicketId);

                if(ticket == null)
                {
                    return NotFound(new { success = false, Message = "Ticket Not Found" });
                }  

                if(ticket.Quantity < request.Quantity)
                {
                    return BadRequest(new { success = false, Message = "Not enought tickets available" });
                }

                await _reservationService.Create(request.TicketId, userId, request.Quantity, request.TicketTotalPrice);

                var message = $"Reservation created. Please review and complete payment within the next 10 minutes to secure your tickets.";

                await _notificationService.Create(new Notification
                {
                    UserId = userId,
                    Message = message,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                });

                await _hubContext.Clients.User(userId.ToString()).SendAsync("UpdateNotificationCountAndData");

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

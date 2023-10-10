using AutoMapper;
using Domain._DTO.Reservation;
using Domain.Entities;
using Infrastructure.Repositories.Events;
using Infrastructure.Repositories.Reservations;
using Infrastructure.Repositories.Tickets;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.AspNetCore.Hosting;
using Services.SendEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Reservation
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITicketTypeRepository _ticketTypesRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IEmailService _emailService;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReservationService(IReservationRepository reservationRepository,ITicketTypeRepository ticketTypesRepository,
            IUserAccountRepository userAccountRepository,
            IEmailService emailService,
            IEventRepository eventRepository,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _reservationRepository = reservationRepository;
            _ticketTypesRepository = ticketTypesRepository;
            _userAccountRepository = userAccountRepository;
            _emailService = emailService;
            _eventRepository = eventRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Domain.Entities.Reservation> Create(int ticketId, int userId, int quantity, double ticketTotalPrice)
        {
            var ticket = await _ticketTypesRepository.GetTicketByIdAsync(ticketId);

            var reservation = new Domain.Entities.Reservation
            {
                Quantity = quantity,
                ReservationTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddMinutes(10),
                IsExpired = false,
                TicketTypeId = ticketId,
                UserAccountId = userId,
                TicketTotalPrice = ticketTotalPrice,
            };

            await _reservationRepository.CreateAsync(reservation);

            ticket.Quantity -= quantity;

            await _ticketTypesRepository.UpdateAsync(ticket);

            await SendPaymentReminderEmail(userId, ticket, reservation,ticketTotalPrice);

            return reservation;
        }

        public async Task<IList<ReservationDto>> GetExpiredReservationsAsync(DateTime currentDate)
        {
            var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync(currentDate);

            return _mapper.Map<List<ReservationDto>>(expiredReservations);
        }

        public async Task SendPaymentReminderEmail(int userId, TicketType ticket, Domain.Entities.Reservation reservation, double ticketTotalPrice)
        {
            var user = await _userAccountRepository.GetById(userId);
            var eventName = await _eventRepository.GetById(ticket.EventId);

            var pathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
            + Path.DirectorySeparatorChar.ToString() + "ReservationReminder.html";

            string subject = "Reservation Reminder";
            string tittle = "Confirm Payment";
            string message = "To ensure you don't miss out, click the button below and complete your payment before your reservation expires: ";
            string body = "";

            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                body = streamReader.ReadToEnd();
            }

            string messageBody = string.Format(body,tittle,user.FirstName,eventName.Name,ticket.Name,reservation.Quantity,reservation.ReservationTime,reservation.ExpirationTime,message,ticketTotalPrice);
           
            try
            {
                await _emailService.SendEmailAsync(user.Email,subject,messageBody);

            }catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<ReservationDto> UpdateAsync(ReservationDto reservationDto)
        {
            var result = await _reservationRepository.UpdateAsync(_mapper.Map<Domain.Entities.Reservation>(reservationDto));

            return _mapper.Map<ReservationDto>(result);
        }
    }
}

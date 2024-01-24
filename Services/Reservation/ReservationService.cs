using AutoMapper;
using Domain._DTO.Reservation;
using Domain.Entities;
using Infrastructure.Repositories.Events;
using Infrastructure.Repositories.Reservations;
using Infrastructure.Repositories.Tickets;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Services.Notification;
using Services.SendEmail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Services.Reservation
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITicketTypeRepository _ticketTypesRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IEmailService _emailService;
        private readonly IEventRepository _eventRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDistributedCache _cache;

        public ReservationService(IReservationRepository reservationRepository, ITicketTypeRepository ticketTypesRepository,
            IUserAccountRepository userAccountRepository,
            IEmailService emailService,
            IEventRepository eventRepository,
            INotificationService notificationService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IDistributedCache cache
            )
        {
            _reservationRepository = reservationRepository;
            _ticketTypesRepository = ticketTypesRepository;
            _userAccountRepository = userAccountRepository;
            _emailService = emailService;
            _eventRepository = eventRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }

        public async Task<Domain.Entities.Reservation> Create(int ticketId,int eventId, int userId, int quantity, double ticketTotalPrice)
        {
            var ticket = await _ticketTypesRepository.GetTicketByIdAsync(ticketId);
            int reservationNumber = await GenerateUniqueReservationNumber();

            var reservation = new Domain.Entities.Reservation
            {
                Quantity = quantity,
                ReservationNumber = reservationNumber,
                ReservationTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddMinutes(10),
                TicketTypeId = ticketId,
                EventId = eventId,
                UserAccountId = userId,
                TicketTotalPrice = ticketTotalPrice,
            };

            await _reservationRepository.CreateAsync(reservation);

            ticket.Quantity -= quantity;

            await _ticketTypesRepository.UpdateAsync(ticket);

            var paymentToken = GeneratePaymentToken();
            await StoreToken(paymentToken, reservation.Id, DateTime.UtcNow.AddMinutes(10));

            await SendPaymentReminderEmail(userId, ticket, reservation, ticketTotalPrice, paymentToken);

            var message = $"Please review and complete payment within the next 10 minutes to secure your tickets. Otherwise your reservation will expire";
            await _notificationService.Create(new Domain.Entities.Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                PaymentLink = $"https://localhost:44331/Payment?token={paymentToken}",
                ReservationId = reservation.Id,
                Type = "Reservation",
            });


            return reservation;
        }

        public async Task<IList<ReservationDto>> GetExpiredReservationsAsync(DateTime currentDate)
        {
            var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync(currentDate);

            return _mapper.Map<List<ReservationDto>>(expiredReservations);
        }

        public async Task SendPaymentReminderEmail(int userId, TicketType ticket, Domain.Entities.Reservation reservation, double ticketTotalPrice, string paymentToken)
        {
            var user = await _userAccountRepository.GetById(userId);
            var eventName = await _eventRepository.GetById(ticket.EventId);

            var pathToFile = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "Templates" + Path.DirectorySeparatorChar.ToString() + "EmailTemplates"
            + Path.DirectorySeparatorChar.ToString() + "ReservationReminder.html";

            string subject = "Reservation Reminder";
            string tittle = "Confirm Payment";
            string message = "To ensure you don't miss out, click the button below and complete your payment before your reservation expires: ";
            string paymentLink = $"https://localhost:44331/Payment?token={paymentToken}";
            string reservationNumber = $"Your reservation number is: {reservation.ReservationNumber}";
            string body = "";

            using (StreamReader streamReader = System.IO.File.OpenText(pathToFile))
            {
                body = streamReader.ReadToEnd();
            }

            string messageBody = string.Format(body, tittle, user.FirstName, eventName.Name, ticket.Name, reservation.Quantity, reservation.ReservationTime, reservation.ExpirationTime, message, ticketTotalPrice.ToString("c"), paymentLink, reservationNumber);

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, messageBody);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<ReservationDto> UpdateAsync(ReservationDto reservationDto)
        {
            var result = await _reservationRepository.UpdateAsync(_mapper.Map<Domain.Entities.Reservation>(reservationDto));

            return _mapper.Map<ReservationDto>(result);
        }

        private string GeneratePaymentToken()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string combinedInput = $"{DateTime.Now.Ticks}The1Best2Strong*Easiest%Secret9Key/In|The[World]?YES:NO";

                byte[] hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private async Task StoreToken(string token, int reservationId, DateTime expirationTime)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            var serializedToken = JsonSerializer.Serialize(new { Token = token, ReservationId = reservationId });
            await _cache.SetStringAsync(token, serializedToken, options);

        }

        private async Task<int> GenerateUniqueReservationNumber()
        {
            int maxAttempts = 10;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                int candidateNumber = await GenerateCandidateReservationNumber();

                bool isUnique = await IsReservationNumberUnique(candidateNumber);

                if (isUnique)
                {
                    return candidateNumber;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique reservation number.");
        }

        private async Task<int> GenerateCandidateReservationNumber()
        {

            Random rand = new Random();
            return rand.Next(11, 99999);
        }

        private async Task<bool> IsReservationNumberUnique(int candidateNumber)
        {
            var exist = await _reservationRepository.ExistsByReservationNumber(candidateNumber);

            return !exist;
        }

        public async Task<ReservationDto> GetByIdWithTicket(int id)
        {
            var reservation = await _reservationRepository
            .GetByIdWithTicket(id, include => include.Include(r => r.TicketTypes));

            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task UpdateReservationStatus(int reservationId, ReservationStatus newStatus)
        {
            var reservation = await _reservationRepository.GetById(reservationId);
            if (reservation != null)
            {
                reservation.Status = newStatus;
                await _reservationRepository.UpdateAsync(reservation);
            }
        }

        public async Task<bool> HasActiveReservationForTickets(int userId, List<int> ticketIds)
        {
            return await _reservationRepository.HasActiveReservationForTickets(userId, ticketIds);
        }

        public async Task<bool> HasCompletedPayment(int userId, int eventId, int ticketId)
        {
            return await _reservationRepository.HasCompletedPayment(userId, eventId, ticketId);
        }
    }
}

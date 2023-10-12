using Domain._DTO.UserAccount;
using Domain.Entities;
using Hangfire;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Tickets;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Events;
using Services.Registration;
using Services.Reservation;
using Services.Role;
using Services.Tickets;
using Services.UserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbExecute
{
    public class DbInitialize : IDbInitialize
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IRoleService _roleService;
        private readonly IUserAccountService _userAccountService;
        public readonly IEventService _eventService;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IReservationService _reservationService;

        public DbInitialize(IServiceProvider serviceProvider,
            IRoleService roleService,
            IUserAccountService userAccountService,
            IEventService eventService,
            ITicketTypeRepository ticketTypeRepository,
            IReservationService reservationService
        )
        {
            _serviceProvider = serviceProvider;
            _userAccountService = userAccountService;
            _roleService = roleService;
            _eventService = eventService;
            _ticketTypeRepository = ticketTypeRepository;
            _reservationService = reservationService;
        }
        public void DbExecute()
        {
            var dbContext = _serviceProvider.GetService<EventManagmentDb>();
            dbContext.Database.Migrate();

            var roleRepository = _serviceProvider.GetService<IRoleRepository>();
            roleRepository.CreateRolesIfNotExists().GetAwaiter().GetResult();

        }

        public async Task CreateAdmin()
        {

            var adminRole = await _roleService.GetRoleByName("Admin");
            if (adminRole == null)
            {
                throw new Exception("Admin role does not exist.");
            }

            var adminUser = await _userAccountService.GetAdminByEmail("admin@admin.com");
            if (adminUser == null)
            {
                var user = new UserAccountCreateDto
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Username = "Admin",
                    Email = "admin@admin.com",
                    Password = "admin123*",
                    PhoneNumber = "044475749",
                    Address = "Prishtina",
                    Gender = 'M',
                    RoleId = adminRole.Id,
                    IsEmailVerified = true

                };


                _userAccountService.Create(user);

            }
        }

        public void DeleteExpiredEvents()
        {
            var expiredEvents = _eventService.GetExpiredEvents();

            foreach (var expiredEvent in expiredEvents)
            {
                expiredEvent.IsActive = false;
                _eventService.UpdateByIsActive(expiredEvent).GetAwaiter().GetResult();
            }
        }

        public async Task UpdateTicketAvailability()
        {
            var currentDate = DateTime.Now;

            var ticketTypes = await _ticketTypeRepository.GetAll();

            foreach (var ticket in ticketTypes)
            {

                var isAvailable = ticket.SaleStartDate <= currentDate && ticket.SaleEndDate >= currentDate;
                ticket.IsAvailable = isAvailable;

                _ticketTypeRepository.Update(ticket);
            }
        }

        public async Task CheckAndUpdateExpiredReservation()
        {
            try
            {
                var expiredReservations = await _reservationService.GetExpiredReservationsAsync(DateTime.Now);

                foreach (var reservation in expiredReservations)
                {
                    if (reservation.Status == ReservationStatus.Paid || reservation.Status == ReservationStatus.PaymentInProgress)
                    {
                        continue;
                    }
                    reservation.Status = ReservationStatus.Expired;
                    await _reservationService.UpdateAsync(reservation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

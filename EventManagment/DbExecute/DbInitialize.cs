using AutoMapper;
using Domain._DTO.Permission;
using Domain._DTO.UserAccount;
using Domain.Entities;
using EventManagment.Hubs;
using Hangfire;
using Infrastructure.Repositories.Permissions;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.Tickets;
using Infrastructure.Repositories.UserAccounts;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Events;
using Services.Notification;
using Services.Permissions;
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
        private readonly IPermissionService _permissionService;
        private readonly IUserAccountService _userAccountService;
        public readonly IEventService _eventService;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IReservationService _reservationService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;

        public DbInitialize(IServiceProvider serviceProvider,
            IRoleService roleService,
            IPermissionService permissionService,
            IUserAccountService userAccountService,
            IEventService eventService,
            ITicketTypeRepository ticketTypeRepository,
            IReservationService reservationService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper
        )
        {
            _serviceProvider = serviceProvider;
            _userAccountService = userAccountService;
            _roleService = roleService;
            _permissionService = permissionService;
            _eventService = eventService;
            _ticketTypeRepository = ticketTypeRepository;
            _reservationService = reservationService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _mapper = mapper;
        }
        public async Task DbExecute()
        {
            var dbContext = _serviceProvider.GetService<EventManagmentDb>();
            dbContext.Database.Migrate();

            var permissionRepository = _serviceProvider.GetService<IPermissionRepository>();
            permissionRepository.SeedPermissionsAsync().GetAwaiter().GetResult();

            var roleRepository = _serviceProvider.GetService<IRoleRepository>();
            roleRepository.SeedRoles().GetAwaiter().GetResult();
            await AssignPermissionsToRoles();
        }

        private async Task AssignPermissionsToRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            var allPermissions = await _permissionService.GetAllPermissionsAsync();
            var dbContext = _serviceProvider.GetService<EventManagmentDb>();

            foreach (var role in roles)
            {
                if(role.Permissions == null)
                {
                    role.Permissions = new List<PermissionDto>();
                }

                List<PermissionDto> rolePermissions = new();

                switch (role.Name)
                {
                    case "Admin":
                        rolePermissions = allPermissions;
                        break;

                    case "Standard User":
                        rolePermissions = new List<PermissionDto>();
                        break;
                    case "EventCreator":
                        rolePermissions = allPermissions
                           .Where(p => p.Name == PermissionType.CreateEvent.ToString() ||
                                p.Name == PermissionType.UpdateEvent.ToString() ||
                                p.Name == PermissionType.DeleteEvent.ToString() ||
                                p.Name == PermissionType.ViewAllEvents.ToString() ||
                                p.Name == PermissionType.CreateTicket.ToString() ||
                                p.Name == PermissionType.UpdateTicket.ToString() ||
                                p.Name == PermissionType.DeleteTicket.ToString() ||
                                p.Name == PermissionType.ViewAllTickets.ToString()).ToList();
                        break;
                    case "EventManager":
                        rolePermissions = allPermissions
                           .Where(p => p.Name == PermissionType.CreateEvent.ToString() ||
                                p.Name == PermissionType.UpdateEvent.ToString() ||
                                p.Name == PermissionType.DeleteEvent.ToString() ||
                                p.Name == PermissionType.ViewAllEvents.ToString() ||
                                p.Name == PermissionType.ApproveEvent.ToString() ||
                                p.Name == PermissionType.CreateTicket.ToString() ||
                                p.Name == PermissionType.UpdateTicket.ToString() ||
                                p.Name == PermissionType.DeleteTicket.ToString() ||
                                p.Name == PermissionType.ViewAllTickets.ToString() ||
                                p.Name == PermissionType.ManageNotifications.ToString()).ToList();
                        break;
                    case "UserManager":
                        rolePermissions = allPermissions
                            .Where(p => p.Name == PermissionType.CreateUser.ToString() ||
                                        p.Name == PermissionType.UpdateUser.ToString() ||
                                        p.Name == PermissionType.DeleteUser.ToString() ||
                                        p.Name == PermissionType.ViewAllUsers.ToString() ||
                                        p.Name == PermissionType.ManageUserRoles.ToString()).ToList();
                        break;
                    case "SystemAdmin":
                        rolePermissions = allPermissions
                            .Where(p => p.Name == PermissionType.ManagePermissions.ToString() ||
                                        p.Name == PermissionType.ConfigureSystem.ToString() ||
                                        p.Name == PermissionType.EditSettings.ToString() ||
                                        p.Name == PermissionType.AccessReports.ToString() ||
                                        p.Name == PermissionType.ViewReports.ToString() ||
                                        p.Name == PermissionType.GenerateReports.ToString()).ToList();
                        break;
                    default:
                        rolePermissions = new List<PermissionDto>();
                        break;
                }

                Console.WriteLine($"Processing Role: {role.Name}");
                role.Permissions.Clear();

                foreach (var permission in rolePermissions)
                {
                    var trackedPermission = await _permissionService.GetByIdAsync(permission.Id);

                    if (trackedPermission != null)
                    {
                        var count = await GetRolePermissionCount(role.Id, permission.Id, dbContext);

                        if (count == 0)
                        {
                            Console.WriteLine($"Adding Permission: {trackedPermission.Name} to Role: {role.Name}");
                            role.Permissions.Add(trackedPermission);
                        }
                    }
                }

                var updatedRole = _mapper.Map<Roles>(role);

                foreach (var permission in updatedRole.Permissions)
                {
                    var existingPermissionEntity = await dbContext.Permission.FindAsync(permission.Id);
                    if (existingPermissionEntity != null)
                    {
                        dbContext.Entry(existingPermissionEntity).State = EntityState.Detached;
                    }
                    dbContext.Entry(permission).State = EntityState.Modified;
                }

                var existingRole = await dbContext.Roles.FindAsync(updatedRole.Id);
                if (existingRole != null)
                {
                    dbContext.Entry(existingRole).State = EntityState.Detached;
                }

                dbContext.Entry(updatedRole).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }

        }

        private async Task<int> GetRolePermissionCount(int roleId, int permissionId, EventManagmentDb dbContext)
        {
            var connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM RolePermission WHERE RolesId = @roleId AND PermissionsId = @permissionId";
                command.Parameters.Add(new SqlParameter("@roleId", roleId));
                command.Parameters.Add(new SqlParameter("@permissionId", permissionId));

                var result = await command.ExecuteScalarAsync();
                await connection.CloseAsync();

                return Convert.ToInt32(result);
            }
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

                    var ticket = await _ticketTypeRepository.GetTicketByIdAsync(reservation.TicketTypeId);
                    ticket.Quantity += reservation.Quantity;

                    await _ticketTypeRepository.UpdateAsync(ticket);

                    var message = $"Your reservation with number: {reservation.ReservationNumber} has expired. The reserved tickets have been released for sale.";
                    await _notificationService.Create(new Domain.Entities.Notification
                    {
                        UserId = reservation.UserAccountId,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        ReservationId = reservation.Id,
                        PaymentLink = String.Empty,
                        Type = "ExpiredReservation",
                    });

                    await _hubContext.Clients.User(reservation.UserAccountId.ToString()).SendAsync("UpdateNotificationCountAndData");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}

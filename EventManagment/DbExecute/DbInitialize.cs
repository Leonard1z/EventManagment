using Domain._DTO.UserAccount;
using Domain.Entities;
using Infrastructure.Repositories.Roles;
using Infrastructure.Repositories.UserAccounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Events;
using Services.Role;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DbInitialize(IServiceProvider serviceProvider,
            IRoleService roleService,
            IUserAccountService userAccountService,
            IEventService eventService,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _serviceProvider = serviceProvider;
            _userAccountService = userAccountService;
            _roleService = roleService;
            _eventService = eventService;
            _webHostEnvironment = webHostEnvironment;
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
                    Email = "admin@admin.com",
                    Password = "admin123*",
                    PhoneNumber = "044475749",
                    Address = "Prishtina",
                    Gender = 'M',
                    RoleId = adminRole.Id

                };


                _userAccountService.Create(user);

            }
        }

        public void DeleteExpiredEvents()
        {
            var expiredEvents = _eventService.GetExpiredEvents();

            foreach (var expiredEvent in expiredEvents)
            {

                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, expiredEvent.Image.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _eventService.Delete(expiredEvent.Id);
            }
        }
    }
}

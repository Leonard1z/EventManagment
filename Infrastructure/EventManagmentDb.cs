using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EventManagmentDb : DbContext
    {
        public EventManagmentDb(DbContextOptions<EventManagmentDb> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AssignedTicket> AssignetTickets { get; set; }
        public DbSet<Permission> Permission { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventManagmentDb).Assembly);

            base.OnModelCreating(modelBuilder);
        }

    }
}

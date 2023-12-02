﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Events
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(EventManagmentDb context) : base(context)
        {
        }

        public IQueryable<Event> GetAllForPagination(string filter, string encryptedId)
        {
            var result = DbSet.Include(x => x.Category)
                .Include(x => x.Registrations)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter) || !string.IsNullOrWhiteSpace(encryptedId))
            {
                if (!string.IsNullOrWhiteSpace(encryptedId))
                {
                    result = result.Where(x => x.Id.ToString().Contains(encryptedId));
                }
                else
                {
                    result = result.Where(x => x.Name.Contains(filter));
                }
            }
            return result;
        }
        public async Task<IList<Event>> GetAllEvents()
        {
            var result = DbSet.Include(x => x.Category)
                .Include(u => u.UserAccount)
                //.Include(x => x.Registrations)
                .AsNoTracking()
                .AsQueryable();

            return await result.ToListAsync();
        }

        public async Task<Event> GetByIdWithCategory(int id)
        {
            var result = DbSet.Include(x => x.Category)
                .Include(x => x.TicketTypes)
                .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);

            return result;
        }

        public IEnumerable<Event> GetExpiredEvents()
        {
            var currentDate = DateTime.Now;
            return DbSet.Where(x => x.EndDate < currentDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<Event>> GetActiveEventsForEventCreator(int userId)
        {
            var result = DbSet.Include(x => x.Category)
                //.Include(x => x.UserAccount)
                //.Include(x => x.Registrations)
                .Where(x => x.UserAccountId == userId && x.IsActive)
                .AsNoTracking()
                .ToListAsync();

            return await result;
        }

        public async Task<IEnumerable<Event>> GetAllByIsActive()
        {
            return await DbSet.Include(x => x.Category)
                .Include(x => x.UserAccount)
                .Include(x => x.Registrations)
                .Where(x => x.IsActive == true)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Event> GetEventDetails(int eventId)
        {
            return await DbSet.Include(x => x.Category)
                               .FirstOrDefaultAsync(x => x.Id == eventId);
        }

        public async Task<int> GetTotalEventCount()
        {
            return await DbSet.CountAsync();
        }

        public async Task<int> GetTotalEventCountForEventCreator(int eventCreatorId)
        {
            return await DbSet.CountAsync(e => e.UserAccountId == eventCreatorId);
        }

        public async Task<IList<Event>> GetUpcomingEvents(int userId, DateTime currentDate)
        {
            return await DbSet.Where(e => e.UserAccountId == userId && e.StartDate > currentDate)
                .ToListAsync();
        }

        public async Task<int> GetTotalUpcomingEventsForEventCreator(int eventCreatorId, DateTime currentDate)
        {
            return await DbSet.CountAsync(e => e.UserAccountId == eventCreatorId && e.StartDate > currentDate);
        }
    }
}

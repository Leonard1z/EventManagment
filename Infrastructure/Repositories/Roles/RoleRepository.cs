using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Repositories.Roles
{
    public class RoleRepository : GenericRepository<Domain.Entities.Roles>, IRoleRepository
    {
        public RoleRepository(EventManagmentDb context) : base(context)
        {

        }

        public Domain.Entities.Roles GetDefaultRole()
        {
            var result = DbSet.FirstOrDefault(x => x.Name == "Standard User");

            return result;
        }

        public Domain.Entities.Roles GetRoleById(int id)
        {
            var result = DbSet.FirstOrDefault(x => x.Id == id);

            return result;
        }

        public async Task SeedRoles()
        {
            var existingRoles = await DbSet.Select(r => r.Name).ToListAsync();

            var rolesToSeed = new List<string> { "Admin", "Standard User", "EventCreator", "EventManager", "UserManager", "SystemAdmin" };


            foreach (var roleName in rolesToSeed)
            {
                if (!existingRoles.Contains(roleName))
                {
                    DbSet.Add(new Domain.Entities.Roles { Name = roleName });
                }
            }
            await SaveAsync();
        }

        public async Task<Domain.Entities.Roles> GetRoleByName(string roleName)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Name == roleName);
        }

        public async Task<List<Permission>> GetPermissionsForRoleAsync(int roleId)
        {
            if (roleId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roleId), "RoleId must be greater than zero.");
            }

            var role = await DbSet.Include(p=>p.Permissions)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(r=>r.Id == roleId);

            if (role == null)
            {
                throw new KeyNotFoundException($"Role with Id {roleId} not found.");
            }

            var permissions = role.Permissions
                               .Select(p => new Permission
                               {
                                   Id = p.Id,
                                   Name = p.Name,
                                   Description = p.Description,
                               }).ToList();

            return permissions;
        }

        public async Task<List<Domain.Entities.Roles>> GetRolesForUserAsync(int userId)
        {
            var userRoles = await DbSet
                                  .Where(r => r.UserAccounts.Any(u => u.Id == userId))
                                  .ToListAsync();

            if (userRoles == null || !userRoles.Any())
            {
                throw new KeyNotFoundException($"Roles for user with ID {userId} not found.");
            }

            return userRoles;
        }
    }
}

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
            var result = DbSet.FirstOrDefault(x => x.Name == "User");

            return result;
        }

        public Domain.Entities.Roles GetRoleById(int id)
        {
            var result = DbSet.FirstOrDefault(x => x.Id == id);

            return result;
        }
        public async Task CreateRolesIfNotExists()
        {
            var roleExists = await DbSet.AnyAsync();

            if (!roleExists)
            {
                var roles = new List<Domain.Entities.Roles>
                {
                    new Domain.Entities.Roles { Name = "Admin" },
                    new Domain.Entities.Roles { Name = "User" }
                };

                await DbSet.AddRangeAsync(roles);
                await SaveAsync();
            }
        }

        public async Task<Domain.Entities.Roles> GetRoleByName(string roleName)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Name == roleName);
        }
    }
}

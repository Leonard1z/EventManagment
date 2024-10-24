using Domain.Entities;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Permissions
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(EventManagmentDb context) : base(context)
        {
                
        }

        public async Task SeedPermissionsAsync()
        {
            var existingPermissions = await DbSet.Select(p=>p.Name).ToListAsync();

            foreach (var permissionType in Enum.GetValues(typeof(PermissionType)))
            {
                var permissionName = permissionType.ToString();

                if(!existingPermissions.Contains(permissionName))
                {
                    DbSet.Add(new Permission
                    {
                        Name = permissionName,
                        Description = $"{permissionName} permission"
                    });
                }
            }

            await SaveAsync();
        }
    }
}

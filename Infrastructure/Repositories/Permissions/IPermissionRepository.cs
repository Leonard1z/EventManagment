using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Permissions
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        Task SeedPermissionsAsync();
    }
}

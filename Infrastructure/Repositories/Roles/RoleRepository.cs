using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Roles
{
    public class RoleRepository : GenericRepository<Domain.Entities.Roles>, IRoleRepository
    {
        public RoleRepository(EventManagmentDb context) : base(context)
        {

        }
    }
}

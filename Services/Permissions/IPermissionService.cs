using Domain._DTO.Permission;
using Domain.Entities;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Permissions
{
    public interface IPermissionService : IService
    {
        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<PermissionDto> GetByIdAsync(int id);
    }
}

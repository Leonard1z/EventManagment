using Domain._DTO.Permission;
using Domain._DTO.Role;
using Domain.Entities;
using Services.Common;

namespace Services.Role
{
    public interface IRoleService : IService
    {
        Task<IEnumerable<RoleDto>> GetAll();
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        RoleDto Create(RoleDto roleDto);
        bool Delete(int id);
        Task<RoleDto> GetById(int id);
        RoleDto Update(RoleDto roleDto);
        RoleDto GetDefaultRole();
        Task<RoleDto> GetRoleByName(string roleName);
        Task<List<PermissionDto>> GetPermissionsForRoleAsync(int id);
        Task<List<RoleDto>> GetRolesForUserAsync(int userId);

    }
}

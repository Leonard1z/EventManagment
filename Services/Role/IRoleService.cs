using Domain._DTO.Role;
using Services.Common;

namespace Services.Role
{
    public interface IRoleService : IService
    {
        Task<IEnumerable<RoleDto>> GetAll();
        RoleDto Create(RoleDto roleDto);
        bool Delete(int id);
        Task<RoleDto> GetById(int id);
        RoleDto Update(RoleDto roleDto);
    }
}

namespace Infrastructure.Repositories.Roles
{
    public interface IRoleRepository : IGenericRepository<Domain.Entities.Roles>
    {
        Domain.Entities.Roles GetDefaultRole();
        Domain.Entities.Roles GetRoleById(int id);
        Task SeedRoles();
        Task<Domain.Entities.Roles> GetRoleByName(string roleName);

    }
}

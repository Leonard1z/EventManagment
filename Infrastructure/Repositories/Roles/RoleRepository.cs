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
    }
}

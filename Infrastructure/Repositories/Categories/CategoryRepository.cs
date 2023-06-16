using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Categories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EventManagmentDb context) : base(context)
        {
        }
        public IQueryable<Category> GetAllForPagination(string filter, string encryptedId)
        {
            var result = DbSet.Include(x => x.Events)
                              .AsNoTracking()
                              .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter) || !string.IsNullOrWhiteSpace(encryptedId))
            {
                if (!string.IsNullOrWhiteSpace(encryptedId))
                {
                    result = result.Where(x => x.Id.ToString().Contains(encryptedId));
                }
                else
                {
                    result = result.Where(x => x.Name.Contains(filter));
                }
            }
            return result;
        }

        public async Task<IList<Category>> GetAllCategories()
        {
            var result = DbSet.Include(x => x.Events)
                              .AsNoTracking()
                              .AsQueryable();

            return await result.ToListAsync();
        }

    }
}

using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Categories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        IQueryable<Category> GetAllForPagination(string filter, string encryptedId);
        Task<IList<Category>> GetAllCategories();
    }
}
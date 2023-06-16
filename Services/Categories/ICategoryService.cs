using Domain._DTO.Category;
using Services.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Categories
{
    public interface ICategoryService : IService
    {
        Task<IEnumerable<CategoryDto>> GetAll();
        CategoryCreateDto Create(CategoryCreateDto categoryCreateDto);
        bool Delete(int id);
        IQueryable<CategoryDto> GetAllForPagination(string filter, string? encryptedId);
        Task<CategoryDto> GetById(int id);
        CategoryDto Update(CategoryDto categoryEditDto);
    }
}
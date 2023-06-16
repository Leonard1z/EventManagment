using AutoMapper;
using Domain._DTO.Category;
using Domain.Entities;
using Infrastructure.Repositories.Categories;

namespace Services.Categories
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public CategoryCreateDto Create(CategoryCreateDto categoryCreateDto)
        {
            var result = _categoryRepository.Create(_mapper.Map<Category>(categoryCreateDto));

            return _mapper.Map<CategoryCreateDto>(result);
        }

        public bool Delete(int id)
        {
            _categoryRepository.Delete(id);

            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAll()
        {
            var result = await _categoryRepository.GetAll();

            return _mapper.Map<List<CategoryDto>>(result.ToList());
        }

        public IQueryable<CategoryDto> GetAllForPagination(string filter, string encryptedId)
        {
            var result = _categoryRepository.GetAllForPagination(filter, encryptedId);

            return _mapper.ProjectTo<CategoryDto>(result);
        }

        public async Task<CategoryDto> GetById(int id)
        {
            return _mapper.Map<CategoryDto>(await _categoryRepository.GetById(id));
        }

        public CategoryDto Update(CategoryDto categoryEditDto)
        {
            var result = _categoryRepository.Update(_mapper.Map<Category>(categoryEditDto));

            return _mapper.Map<CategoryDto>(result);
        }


    }
}

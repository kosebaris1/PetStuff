using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Interfaces.CategoryInterface
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByNameAsync(string name);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}

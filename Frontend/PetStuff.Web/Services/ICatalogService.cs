using PetStuff.Web.Models;

namespace PetStuff.Web.Services
{
    public interface ICatalogService
    {
        Task<List<ProductListViewModel>> GetProductsAsync(string token);
        Task<ProductViewModel?> GetProductByIdAsync(int id, string token);
        Task<List<ProductListViewModel>> GetProductsByCategoryAsync(int categoryId, string token);
        Task<List<CategoryViewModel>> GetCategoriesAsync(string token);
        Task<List<BrandViewModel>> GetBrandsAsync(string token);
        Task<bool> CreateProductAsync(CreateProductViewModel product, string token);
        Task<bool> UpdateProductAsync(int id, UpdateProductViewModel product, string token);
        Task<bool> DeleteProductAsync(int id, string token);
    }
}


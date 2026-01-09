using PetStuff.Web.Models;

namespace PetStuff.Web.Services
{
    public interface ICatalogService
    {
        Task<List<ProductListViewModel>> GetProductsAsync(string? token = null);
        Task<ProductViewModel?> GetProductByIdAsync(int id, string? token = null);
        Task<List<ProductListViewModel>> GetProductsByCategoryAsync(int categoryId, string? token = null);
        Task<List<ProductListViewModel>> GetProductsByBrandAsync(int brandId, string? token = null);
        Task<List<CategoryViewModel>> GetCategoriesAsync(string? token = null);
        Task<CategoryViewModel?> GetCategoryByIdAsync(int id, string token);
        Task<List<BrandViewModel>> GetBrandsAsync(string? token = null);
        Task<BrandViewModel?> GetBrandByIdAsync(int id, string token);
        Task<bool> CreateProductAsync(CreateProductViewModel product, string token);
        Task<bool> UpdateProductAsync(int id, UpdateProductViewModel product, string token);
        Task<bool> DeleteProductAsync(int id, string token);
        Task<bool> CreateCategoryAsync(CreateCategoryViewModel category, string token);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryViewModel category, string token);
        Task<bool> DeleteCategoryAsync(int id, string token);
        Task<bool> CreateBrandAsync(CreateBrandViewModel brand, string token);
        Task<bool> UpdateBrandAsync(int id, UpdateBrandViewModel brand, string token);
        Task<bool> DeleteBrandAsync(int id, string token);
    }
}


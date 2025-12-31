using PetStuff.Catalog.Domain.Entities;

namespace PetStuff.Catalog.Application.Interfaces.BrandInterface
{
    public interface IBrandRepository
    {
        Task<Brand> GetByIdAsync(int id);
        Task<List<Brand>> GetAllAsync();
        Task<Brand> GetByNameAsync(string name);
        Task CreateBrandAsync(Brand brand);
        Task UpdateBrandAsync(Brand brand);
        Task DeleteBrandAsync(Brand brand);
    }
}





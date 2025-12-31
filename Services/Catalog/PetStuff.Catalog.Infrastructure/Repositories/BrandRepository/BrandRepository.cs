using Microsoft.EntityFrameworkCore;
using PetStuff.Catalog.Application.Interfaces.BrandInterface;
using PetStuff.Catalog.Domain.Entities;
using PetStuff.Catalog.Infrastructure.Context;

namespace PetStuff.Catalog.Infrastructure.Repositories.BrandRepository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly CatalogDbContext _context;

        public BrandRepository(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByNameAsync(string name)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());
        }

        public async Task CreateBrandAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }
    }
}





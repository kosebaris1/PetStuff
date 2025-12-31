using Microsoft.EntityFrameworkCore;
using PetStuff.Catalog.Application.Interfaces.ProductInterface;
using PetStuff.Catalog.Domain.Entities;
using PetStuff.Catalog.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStuff.Catalog.Infrastructure.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogDbContext _context;

        public ProductRepository(CatalogDbContext catalogDbContext)
        {
            _context = catalogDbContext;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // Mevcut product'ı ve image'larını yükle
            var existingProduct = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with id {product.Id} not found.");
            }

            // Product bilgilerini güncelle
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.IsActive = product.IsActive;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.BrandId = product.BrandId;
            existingProduct.UpdatedDate = product.UpdatedDate;

            // Image'ları güncelle - Önce mevcut image'ları sil
            if (existingProduct.Images != null && existingProduct.Images.Any())
            {
                _context.Set<ProductImage>().RemoveRange(existingProduct.Images);
            }

            // Yeni image'ları ekle
            if (product.Images != null && product.Images.Any())
            {
                foreach (var image in product.Images)
                {
                    existingProduct.Images ??= new List<ProductImage>();
                    existingProduct.Images.Add(new ProductImage
                    {
                        ImageUrl = image.ImageUrl,
                        IsMain = image.IsMain,
                        ProductId = existingProduct.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}

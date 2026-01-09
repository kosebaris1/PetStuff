using Microsoft.Extensions.Configuration;
using PetStuff.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PetStuff.Web.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public CatalogService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            var baseUrl = _configuration["ApiEndpoints:CatalogApi"] ?? "";
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("CatalogApi endpoint is not configured in appsettings.json");
            }
            
            _httpClient.BaseAddress = new Uri(baseUrl);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<ProductListViewModel>> GetProductsAsync(string? token = null)
        {
            try
            {
                // Clear previous headers
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                
                // Set authorization only if token is provided
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                
                var response = await _httpClient.GetAsync("/api/catalog/products");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductListViewModel>>(json, _jsonOptions) ?? new List<ProductListViewModel>();
                    return products;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Catalog API Error: {response.StatusCode} - {errorContent}");
                    // Log error for debugging
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CatalogService.GetProductsAsync Exception: {ex.Message}");
            }
            
            return new List<ProductListViewModel>();
        }

        public async Task<ProductViewModel?> GetProductByIdAsync(int id, string? token = null)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await _httpClient.GetAsync($"/api/catalog/products/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductViewModel>(json, _jsonOptions);
            }
            
            return null;
        }

        public async Task<List<ProductListViewModel>> GetProductsByCategoryAsync(int categoryId, string? token = null)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await _httpClient.GetAsync($"/api/catalog/products/category/{categoryId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ProductListViewModel>>(json, _jsonOptions) ?? new List<ProductListViewModel>();
            }
            
            return new List<ProductListViewModel>();
        }

        public async Task<List<ProductListViewModel>> GetProductsByBrandAsync(int brandId, string? token = null)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await _httpClient.GetAsync($"/api/catalog/products/brand/{brandId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ProductListViewModel>>(json, _jsonOptions) ?? new List<ProductListViewModel>();
            }
            
            return new List<ProductListViewModel>();
        }

        public async Task<List<CategoryViewModel>> GetCategoriesAsync(string? token = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                
                var response = await _httpClient.GetAsync("/api/catalog/categories");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<CategoryViewModel>>(json, _jsonOptions) ?? new List<CategoryViewModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Catalog API Categories Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CatalogService.GetCategoriesAsync Exception: {ex.Message}");
            }
            
            return new List<CategoryViewModel>();
        }

        public async Task<List<BrandViewModel>> GetBrandsAsync(string? token = null)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                
                var response = await _httpClient.GetAsync("/api/catalog/brands");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<BrandViewModel>>(json, _jsonOptions) ?? new List<BrandViewModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Catalog API Brands Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CatalogService.GetBrandsAsync Exception: {ex.Message}");
            }
            
            return new List<BrandViewModel>();
        }

        public async Task<bool> CreateProductAsync(CreateProductViewModel product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(product, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/catalog/products", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductViewModel product, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(product, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/catalog/products/{id}", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/catalog/products/{id}");
            
            return response.IsSuccessStatusCode;
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/catalog/categories/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CategoryViewModel>(json, _jsonOptions);
            }
            
            return null;
        }

        public async Task<BrandViewModel?> GetBrandByIdAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/catalog/brands/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BrandViewModel>(json, _jsonOptions);
            }
            
            return null;
        }

        public async Task<bool> CreateCategoryAsync(CreateCategoryViewModel category, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(category, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/catalog/categories", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryViewModel category, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(category, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/catalog/categories/{id}", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/catalog/categories/{id}");
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateBrandAsync(CreateBrandViewModel brand, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(brand, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/catalog/brands", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateBrandAsync(int id, UpdateBrandViewModel brand, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null }; // PascalCase for API
            var json = JsonSerializer.Serialize(brand, jsonOptions);
            var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/catalog/brands/{id}", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBrandAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"/api/catalog/brands/{id}");
            
            return response.IsSuccessStatusCode;
        }
    }
}


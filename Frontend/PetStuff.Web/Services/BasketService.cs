using Microsoft.Extensions.Configuration;
using PetStuff.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PetStuff.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public BasketService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            var baseUrl = _configuration["ApiEndpoints:BasketApi"] ?? "";
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("BasketApi endpoint is not configured in appsettings.json");
            }
            
            _httpClient.BaseAddress = new Uri(baseUrl);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<BasketViewModel?> GetBasketAsync(string token)
        {
            try
            {
                // Clear previous headers and set new authorization
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.GetAsync("/api/basket");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<BasketViewModel>(json, _jsonOptions);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Basket API GetBasketAsync Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BasketService.GetBasketAsync Exception: {ex.Message}");
            }
            
            return null;
        }

        public async Task<bool> AddItemToBasketAsync(AddToBasketViewModel item, string token)
        {
            try
            {
                // Clear previous headers and set new authorization
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var json = JsonSerializer.Serialize(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/basket/items", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Basket API AddItemToBasketAsync Error: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BasketService.AddItemToBasketAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateItemQuantityAsync(int productId, int quantity, string token)
        {
            try
            {
                // Clear previous headers and set new authorization
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var json = JsonSerializer.Serialize(new { Quantity = quantity });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/basket/items/{productId}/quantity", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Basket API UpdateItemQuantityAsync Error: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BasketService.UpdateItemQuantityAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveItemFromBasketAsync(int productId, string token)
        {
            try
            {
                // Clear previous headers and set new authorization
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.DeleteAsync($"/api/basket/items/{productId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Basket API RemoveItemFromBasketAsync Error: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BasketService.RemoveItemFromBasketAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ClearBasketAsync(string token)
        {
            try
            {
                // Clear previous headers and set new authorization
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.DeleteAsync("/api/basket");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Basket API ClearBasketAsync Error: {response.StatusCode} - {errorContent}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BasketService.ClearBasketAsync Exception: {ex.Message}");
                return false;
            }
        }
    }
}


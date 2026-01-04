using Microsoft.Extensions.Configuration;
using PetStuff.Order.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PetStuff.Order.Infrastructure.Services
{
    public class CatalogServiceClient : ICatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CatalogServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var baseUrl = _configuration["ApiEndpoints:CatalogApi"] ?? "";
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("CatalogApi endpoint is not configured in appsettings.json");
            }

            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<bool> CheckStockAsync(List<OrderItemStockRequest> items, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Her bir item için stok kontrolü yap
                foreach (var item in items)
                {
                    var checkRequest = new
                    {
                        quantity = item.Quantity
                    };
                    
                    var checkJson = JsonSerializer.Serialize(checkRequest, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    var checkContent = new StringContent(checkJson, Encoding.UTF8, "application/json");
                    var checkResponse = await _httpClient.PostAsync($"/api/products/{item.ProductId}/check-stock", checkContent);
                    
                    if (!checkResponse.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    var checkResultJson = await checkResponse.Content.ReadAsStringAsync();
                    var checkResult = JsonSerializer.Deserialize<CheckStockResult>(checkResultJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (checkResult == null || !checkResult.IsAvailable)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CatalogServiceClient.CheckStockAsync Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ReduceStockAsync(List<OrderItemStockRequest> items, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var requestBody = items.Select(item => new
                {
                    productId = item.ProductId,
                    quantity = item.Quantity
                }).ToList();

                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/products/reduce-stock-for-order", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CatalogServiceClient.ReduceStockAsync Exception: {ex.Message}");
                return false;
            }
        }

        private class CheckStockResult
        {
            public bool IsAvailable { get; set; }
            public int ProductId { get; set; }
            public int RequestedQuantity { get; set; }
        }
    }
}

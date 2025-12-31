using Microsoft.Extensions.Configuration;
using PetStuff.Web.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PetStuff.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiEndpoints:OrderApi"] ?? "");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<OrderViewModel>> GetOrdersAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/orders");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<JsonElement>>(json, _jsonOptions) ?? new List<JsonElement>();
                
                return orders.Select(o => new OrderViewModel
                {
                    Id = o.GetProperty("id").GetInt32(),
                    UserId = o.GetProperty("userId").GetString() ?? "",
                    OrderDate = o.GetProperty("orderDate").GetDateTime(),
                    TotalPrice = o.GetProperty("totalPrice").GetDecimal(),
                    Status = GetStatusName(o.GetProperty("status").GetInt32()),
                    ShippingAddress = o.GetProperty("shippingAddress").GetString() ?? "",
                    ShippingCity = o.TryGetProperty("shippingCity", out var city) ? city.GetString() : null,
                    ShippingCountry = o.TryGetProperty("shippingCountry", out var country) ? country.GetString() : null,
                    ShippingZipCode = o.TryGetProperty("shippingZipCode", out var zip) ? zip.GetString() : null,
                    Items = o.GetProperty("items").EnumerateArray().Select(item => new OrderItemViewModel
                    {
                        Id = item.GetProperty("id").GetInt32(),
                        ProductId = item.GetProperty("productId").GetInt32(),
                        ProductName = item.GetProperty("productName").GetString() ?? "",
                        Price = item.GetProperty("price").GetDecimal(),
                        Quantity = item.GetProperty("quantity").GetInt32(),
                        ProductImageUrl = item.TryGetProperty("productImageUrl", out var img) ? img.GetString() : null
                    }).ToList()
                }).ToList();
            }
            
            return new List<OrderViewModel>();
        }

        public async Task<OrderViewModel?> GetOrderByIdAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"/api/orders/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var o = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
                
                return new OrderViewModel
                {
                    Id = o.GetProperty("id").GetInt32(),
                    UserId = o.GetProperty("userId").GetString() ?? "",
                    OrderDate = o.GetProperty("orderDate").GetDateTime(),
                    TotalPrice = o.GetProperty("totalPrice").GetDecimal(),
                    Status = GetStatusName(o.GetProperty("status").GetInt32()),
                    ShippingAddress = o.GetProperty("shippingAddress").GetString() ?? "",
                    ShippingCity = o.TryGetProperty("shippingCity", out var city) ? city.GetString() : null,
                    ShippingCountry = o.TryGetProperty("shippingCountry", out var country) ? country.GetString() : null,
                    ShippingZipCode = o.TryGetProperty("shippingZipCode", out var zip) ? zip.GetString() : null,
                    Items = o.GetProperty("items").EnumerateArray().Select(item => new OrderItemViewModel
                    {
                        Id = item.GetProperty("id").GetInt32(),
                        ProductId = item.GetProperty("productId").GetInt32(),
                        ProductName = item.GetProperty("productName").GetString() ?? "",
                        Price = item.GetProperty("price").GetDecimal(),
                        Quantity = item.GetProperty("quantity").GetInt32(),
                        ProductImageUrl = item.TryGetProperty("productImageUrl", out var img) ? img.GetString() : null
                    }).ToList()
                };
            }
            
            return null;
        }

        public async Task<OrderViewModel?> CreateOrderAsync(CreateOrderViewModel order, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/orders", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var o = JsonSerializer.Deserialize<JsonElement>(responseJson, _jsonOptions);
                
                return new OrderViewModel
                {
                    Id = o.GetProperty("id").GetInt32(),
                    UserId = o.GetProperty("userId").GetString() ?? "",
                    OrderDate = o.GetProperty("orderDate").GetDateTime(),
                    TotalPrice = o.GetProperty("totalPrice").GetDecimal(),
                    Status = GetStatusName(o.GetProperty("status").GetInt32()),
                    ShippingAddress = o.GetProperty("shippingAddress").GetString() ?? "",
                    ShippingCity = o.TryGetProperty("shippingCity", out var city) ? city.GetString() : null,
                    ShippingCountry = o.TryGetProperty("shippingCountry", out var country) ? country.GetString() : null,
                    ShippingZipCode = o.TryGetProperty("shippingZipCode", out var zip) ? zip.GetString() : null,
                    Items = o.GetProperty("items").EnumerateArray().Select(item => new OrderItemViewModel
                    {
                        Id = item.GetProperty("id").GetInt32(),
                        ProductId = item.GetProperty("productId").GetInt32(),
                        ProductName = item.GetProperty("productName").GetString() ?? "",
                        Price = item.GetProperty("price").GetDecimal(),
                        Quantity = item.GetProperty("quantity").GetInt32(),
                        ProductImageUrl = item.TryGetProperty("productImageUrl", out var img) ? img.GetString() : null
                    }).ToList()
                };
            }
            
            return null;
        }

        private string GetStatusName(int status)
        {
            return status switch
            {
                0 => "Pending",
                1 => "Confirmed",
                2 => "Processing",
                3 => "Shipped",
                4 => "Delivered",
                5 => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}


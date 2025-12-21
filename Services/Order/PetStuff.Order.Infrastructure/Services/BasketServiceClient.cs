using Microsoft.Extensions.Configuration;
using PetStuff.Order.Application.DTOs;
using PetStuff.Order.Application.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetStuff.Order.Infrastructure.Services
{
    public class BasketServiceClient : IBasketServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public BasketServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<BasketServiceDto?> GetBasketAsync(string userId, string token)
        {
            var baseUrl = _configuration["BasketService:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            
            try
            {
                var response = await _httpClient.GetAsync($"{baseUrl}/api/basket");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<BasketServiceDto>(json, _jsonOptions);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ClearBasketAsync(string userId, string token)
        {
            var baseUrl = _configuration["BasketService:BaseUrl"];
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            
            try
            {
                var response = await _httpClient.DeleteAsync($"{baseUrl}/api/basket");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}


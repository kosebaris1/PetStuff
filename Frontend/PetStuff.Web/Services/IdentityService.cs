using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PetStuff.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IdentityService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiEndpoints:IdentityServer"] ?? "");
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var requestBody = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "password"),
                new("client_id", "petstuff.client"),
                new("client_secret", "petstuff_secret"),
                new("username", username),
                new("password", password),
                new("scope", "catalog.api basket.api order.api inventory.api openid profile email")
            };

            var formUrlEncodedContent = new FormUrlEncodedContent(requestBody);
            var response = await _httpClient.PostAsync("/connect/token", formUrlEncodedContent);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"IdentityService Response: {json}");
                
                var tokenData = JsonSerializer.Deserialize<JsonElement>(json);
                
                if (tokenData.TryGetProperty("access_token", out var accessToken))
                {
                    var token = accessToken.GetString();
                    Console.WriteLine($"Token extracted: {(token != null && token.Length > 50 ? token.Substring(0, 50) + "..." : token)}");
                    
                    // Token'ın JWT formatında olduğunu kontrol et (ey ile başlamalı)
                    if (token != null && token.StartsWith("ey"))
                    {
                        return token;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Token does not start with 'ey'. Token format: {token?.Substring(0, Math.Min(20, token?.Length ?? 0))}");
                    }
                }
            }

            return null;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string? fullName)
        {
            var registerData = new
            {
                Username = username,
                Email = email,
                Password = password,
                FullName = fullName
            };

            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/register", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterAdminAsync(string username, string email, string password, string? fullName)
        {
            var registerData = new
            {
                Username = username,
                Email = email,
                Password = password,
                FullName = fullName
            };

            var json = JsonSerializer.Serialize(registerData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/register-admin", content);

            return response.IsSuccessStatusCode;
        }
    }
}


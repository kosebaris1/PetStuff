using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PetStuff.Email.Api.Services
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityServiceClient> _logger;

        public IdentityServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<IdentityServiceClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GetUserEmailAsync(string userId)
        {
            try
            {
                var baseUrl = _configuration["IdentityServer:BaseUrl"] ?? "https://localhost:7120";
                var url = $"{baseUrl}/api/Account/user/{userId}/email";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get user email for userId: {userId}. Status: {response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<EmailResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Email;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user email for userId: {userId}");
                return null;
            }
        }

        private class EmailResponse
        {
            public string? Email { get; set; }
        }
    }
}


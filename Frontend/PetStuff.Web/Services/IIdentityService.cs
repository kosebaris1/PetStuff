namespace PetStuff.Web.Services
{
    public interface IIdentityService
    {
        Task<string?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password, string? fullName);
        Task<bool> RegisterAdminAsync(string username, string email, string password, string? fullName);
    }
}


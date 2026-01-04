namespace PetStuff.Email.Api.Services
{
    public interface IIdentityServiceClient
    {
        Task<string?> GetUserEmailAsync(string userId);
    }
}


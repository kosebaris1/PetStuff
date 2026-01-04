namespace PetStuff.Email.Api.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmedEmailAsync(string toEmail, int orderId);
        Task SendOrderShippedEmailAsync(string toEmail, int orderId);
        Task SendOrderDeliveredEmailAsync(string toEmail, int orderId);
    }
}


using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PetStuff.Email.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendOrderConfirmedEmailAsync(string toEmail, int orderId)
        {
            var subject = "Siparişiniz Onaylandı";
            var body = $@"
                <html>
                <body>
                    <h2>Merhaba,</h2>
                    <p>Siparişiniz (#{orderId}) başarıyla onaylandı.</p>
                    <p>Ürünleriniz hazırlanıyor ve en kısa sürede kargoya verilecektir.</p>
                    <p>Teşekkür ederiz.</p>
                    <p>PetStuff Ekibi</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderShippedEmailAsync(string toEmail, int orderId)
        {
            var subject = "Siparişiniz Kargoya Verildi";
            var body = $@"
                <html>
                <body>
                    <h2>Merhaba,</h2>
                    <p>Siparişiniz (#{orderId}) kargoya verilmiştir.</p>
                    <p>En yakın zamanda tarafınıza ulaştırılacaktır.</p>
                    <p>Takip için sipariş numaranızı kullanabilirsiniz.</p>
                    <p>Teşekkür ederiz.</p>
                    <p>PetStuff Ekibi</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderDeliveredEmailAsync(string toEmail, int orderId)
        {
            var subject = "Siparişiniz Teslim Edildi";
            var body = $@"
                <html>
                <body>
                    <h2>Merhaba,</h2>
                    <p>Siparişiniz (#{orderId}) başarıyla teslim edilmiştir.</p>
                    <p>PetStuff'i tercih ettiğiniz için teşekkür ederiz.</p>
                    <p>Yeni alışverişlerde görüşmek üzere!</p>
                    <p>PetStuff Ekibi</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"] ?? "PetStuff",
                    _configuration["Email:FromAddress"] ?? "noreply@petstuff.com"));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                    int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["Email:SmtpUser"],
                    _configuration["Email:SmtpPassword"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toEmail} for order status notification");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
                throw;
            }
        }
    }
}


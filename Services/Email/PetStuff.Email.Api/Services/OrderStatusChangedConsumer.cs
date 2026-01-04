using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PetStuff.Email.Api.Services
{
    public class OrderStatusChangedConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OrderStatusChangedConsumer> _logger;
        private const string ExchangeName = "order_events";
        private const string QueueName = "order_status_changed";

        public OrderStatusChangedConsumer(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<OrderStatusChangedConsumer> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Exchange oluştur
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);

            // Queue oluştur
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);

            // Queue'yu exchange'e bağla
            _channel.QueueBind(QueueName, ExchangeName, "order.status.changed");

            // QoS ayarları
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderStatusChangedEvent>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (orderEvent != null)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            var identityServiceClient = scope.ServiceProvider.GetRequiredService<IIdentityServiceClient>();
                            await ProcessOrderStatusChangedAsync(orderEvent, emailService, identityServiceClient);
                        }
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing order status changed event: {message}");
                    _channel.BasicNack(ea.DeliveryTag, false, true); // Requeue
                }
            };

            _channel.BasicConsume(QueueName, autoAck: false, consumer);

            _logger.LogInformation("OrderStatusChangedConsumer started. Waiting for messages...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessOrderStatusChangedAsync(
            OrderStatusChangedEvent orderEvent, 
            IEmailService emailService, 
            IIdentityServiceClient identityServiceClient)
        {
            _logger.LogInformation($"Processing order status changed event: OrderId={orderEvent.OrderId}, Status={orderEvent.Status}, UserId={orderEvent.UserId}");

            // Email bilgisini al
            string? email = orderEvent.Email;

            // Eğer email yoksa, Identity Server'dan çek
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogInformation($"Email not in event, fetching from Identity Server for userId: {orderEvent.UserId}");
                email = await identityServiceClient.GetUserEmailAsync(orderEvent.UserId);
                _logger.LogInformation($"Email from Identity Server: {(string.IsNullOrEmpty(email) ? "NOT FOUND" : email)}");
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($"Could not find email for userId: {orderEvent.UserId}, skipping email send");
                return;
            }

            // Status'e göre email gönder (ToUpperInvariant kullanarak Türkçe karakter sorununu önle)
            var normalizedStatus = orderEvent.Status.ToUpperInvariant();
            _logger.LogInformation($"Sending email for status: {orderEvent.Status} (normalized: {normalizedStatus})");

            switch (normalizedStatus)
            {
                case "CONFIRMED":
                    _logger.LogInformation($"Sending Confirmed email to {email} for order {orderEvent.OrderId}");
                    await emailService.SendOrderConfirmedEmailAsync(email, orderEvent.OrderId);
                    _logger.LogInformation($"Confirmed email sent successfully");
                    break;
                case "SHIPPED":
                    _logger.LogInformation($"Sending Shipped email to {email} for order {orderEvent.OrderId}");
                    await emailService.SendOrderShippedEmailAsync(email, orderEvent.OrderId);
                    _logger.LogInformation($"Shipped email sent successfully");
                    break;
                case "DELIVERED":
                    _logger.LogInformation($"Sending Delivered email to {email} for order {orderEvent.OrderId}");
                    await emailService.SendOrderDeliveredEmailAsync(email, orderEvent.OrderId);
                    _logger.LogInformation($"Delivered email sent successfully");
                    break;
                default:
                    _logger.LogWarning($"Unknown order status: '{orderEvent.Status}' (normalized: '{normalizedStatus}'), cannot send email");
                    break;
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }

    public class OrderStatusChangedEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime Timestamp { get; set; }
    }
}


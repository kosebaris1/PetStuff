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
        private readonly IEmailService _emailService;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly ILogger<OrderStatusChangedConsumer> _logger;
        private const string ExchangeName = "order_events";
        private const string QueueName = "order_status_changed";

        public OrderStatusChangedConsumer(
            IConfiguration configuration,
            IEmailService emailService,
            IIdentityServiceClient identityServiceClient,
            ILogger<OrderStatusChangedConsumer> logger)
        {
            _emailService = emailService;
            _identityServiceClient = identityServiceClient;
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
                        await ProcessOrderStatusChangedAsync(orderEvent);
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

        private async Task ProcessOrderStatusChangedAsync(OrderStatusChangedEvent orderEvent)
        {
            _logger.LogInformation($"Processing order status changed event: OrderId={orderEvent.OrderId}, Status={orderEvent.Status}, UserId={orderEvent.UserId}");

            // Email bilgisini al
            string? email = orderEvent.Email;

            // Eğer email yoksa, Identity Server'dan çek
            if (string.IsNullOrEmpty(email))
            {
                email = await _identityServiceClient.GetUserEmailAsync(orderEvent.UserId);
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning($"Could not find email for userId: {orderEvent.UserId}");
                return;
            }

            // Status'e göre email gönder
            switch (orderEvent.Status.ToUpper())
            {
                case "CONFIRMED":
                    await _emailService.SendOrderConfirmedEmailAsync(email, orderEvent.OrderId);
                    break;
                case "SHIPPED":
                    await _emailService.SendOrderShippedEmailAsync(email, orderEvent.OrderId);
                    break;
                case "DELIVERED":
                    await _emailService.SendOrderDeliveredEmailAsync(email, orderEvent.OrderId);
                    break;
                default:
                    _logger.LogWarning($"Unknown order status: {orderEvent.Status}");
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


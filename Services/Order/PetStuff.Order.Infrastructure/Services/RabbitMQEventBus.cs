using Microsoft.Extensions.Configuration;
using PetStuff.Order.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PetStuff.Order.Infrastructure.Services
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "order_events";
        private const string QueueName = "order_status_changed";

        public RabbitMQEventBus(IConfiguration configuration)
        {
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
        }

        public void PublishOrderStatusChangedEvent(int orderId, string userId, string status, string? email = null)
        {
            var message = new
            {
                OrderId = orderId,
                UserId = userId,
                Status = status,
                Email = email,
                Timestamp = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: "order.status.changed",
                basicProperties: properties,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}


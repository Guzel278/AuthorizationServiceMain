using System.Text;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace AuthorizationService.Infrastructure.MessageBroker
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly ILogger<RabbitMqService> _logger;
        private readonly IConnection _connection;

        public RabbitMqService(ILogger<RabbitMqService> logger, RabbitMqConnectionManager connectionManager)
        {
            _logger = logger;
            _connection = connectionManager.GetConnection();
        }

        public void Send<T>(T message, string queue)
        {
            _logger.LogInformation("Sending message of type {0} to queue {1}", typeof(T).Name, queue);

            using var channel = _connection.CreateModel();

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: queue,
                                 basicProperties: properties,
                                 body: body);

            _logger.LogInformation("Message sent to queue {0}: {1}", queue, typeof(T).Name);
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing RabbitMqService...");
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

